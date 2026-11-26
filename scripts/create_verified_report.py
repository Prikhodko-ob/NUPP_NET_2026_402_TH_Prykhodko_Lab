from pathlib import Path
import html, os, sys, textwrap
from playwright.sync_api import sync_playwright
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import A4
from reportlab.lib.utils import ImageReader
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

branch=os.environ.get("LAB_BRANCH","lab3")
number=int(branch.replace("lab","").replace("-",""))
out=Path("verification")
out.mkdir(exist_ok=True)
title={3:"Entity Framework Core, SQLite та MongoDB",4:"ASP.NET Core Web API та MVC",5:"ASP.NET Core Identity: автентифікація та авторизація"}[number]
font="/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
pdfmetrics.registerFont(TTFont("Ukrainian",font))
pdf=canvas.Canvas(str(out/f"Lab{number}_Report.pdf"),pagesize=A4)
W,H=A4
def heading(name):
    pdf.setFont("Ukrainian",11)
    pdf.drawString(42,H-44,f"Лабораторна робота №{number}  |  ConstructionCompany")
    pdf.line(42,H-54,W-42,H-54)
    pdf.setFont("Ukrainian",13)
    pdf.drawString(42,H-78,name)
    pdf.setFont("Ukrainian",9)
    pdf.drawString(42,32,f"Приходько О. Б. • група 402 • сторінка {pdf.getPageNumber()}")
def image_page(name,file):
    heading(name)
    im=ImageReader(str(file))
    iw,ih=im.getSize()
    factor=min((W-84)/iw,(H-150)/ih)
    pdf.drawImage(im,42,H-94-ih*factor,width=iw*factor,height=ih*factor,mask="auto")
    pdf.showPage()

heading("Звіт про автоматизовану перевірку")
pdf.setFont("Ukrainian",19)
for i,line in enumerate(textwrap.wrap(title,42)):
    pdf.drawString(42,H-130-i*28,line)
pdf.setFont("Ukrainian",11)
lines=[
    "Студент: Приходько О. Б.   Група: 402",
    "Тема проєкту: будівельна компанія",
    f"Гілка: {branch}",
    f"Запуск GitHub Actions: {os.environ.get('GITHUB_RUN_ID','local')}",
    f"Коміт: {os.environ.get('GITHUB_SHA','local')[:16]}",
    "Джерело доказів: журнали та дані реального запуску CI.",
    "Знімки сформовано браузером із фактичного виводу;",
    "це не імітація DB Browser або Postman.",
    "Успішний запуск компіляції, тестів та перевірок",
    "є передумовою створення цього звіту."
]
if number==4: lines += ["MVC: реалізацію включено, окремий UI-тест не виконано."]
if number==5: lines += ["Хмарне розгортання не виконано."]
for i,line in enumerate(lines): pdf.drawString(42,H-222-i*21,line)
pdf.showPage()
sections=[("console.txt","Результати консольного застосунку"),("sqlite.txt","Фактичні таблиці SQLite та перевірка цілісності"),("mongo.txt","Фактичні документи MongoDB")]
if number>3: sections.append(("http.txt","Фактичні HTTP-запити та відповіді"))
with sync_playwright() as p:
    browser=p.chromium.launch(headless=True)
    page=browser.new_page(viewport={"width":1000,"height":760},device_scale_factor=1)
    for filename,name in sections:
        source=out/filename
        if not source.exists(): raise RuntimeError(f"Missing evidence: {source}")
        lines=[]
        for line in source.read_text(errors="replace").splitlines():
            lines.extend(textwrap.wrap(line,width=94,replace_whitespace=False,drop_whitespace=False) or [""])
        for i in range(0,len(lines),28):
            part=lines[i:i+28]
            page.set_content("<!doctype html><meta charset=utf-8><style>body{font:16px sans-serif;color:#182235;padding:22px;background:#fff}h2{font-size:20px}pre{font:15px monospace;line-height:1.5;background:#f3f6fa;padding:16px;white-space:pre-wrap;overflow-wrap:anywhere}</style><h2>"+html.escape(name)+"</h2><p>GitHub Actions — фактичний вивід, фрагмент "+str(i//28+1)+"</p><pre>"+html.escape("\n".join(part))+"</pre>")
            png=out/(source.stem+"-"+str(i//28+1)+".png")
            page.screenshot(path=str(png),full_page=True)
            image_page(name,png)
    browser.close()
if number==3:
    image_page("ERD — структура реляційної бази",Path("Evidence/ERD.png"))
if (out/"swagger.png").exists():
    image_page("Swagger UI — фактичний запуск",out/"swagger.png")
pdf.save()
