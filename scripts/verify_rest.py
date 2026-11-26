import json, os, urllib.request, urllib.error, uuid
base = "http://127.0.0.1:5080"
def request(method, path, payload=None, token=None, expected=200):
    headers = {"Content-Type":"application/json"}
    if token: headers["Authorization"] = "Bearer " + token
    data = json.dumps(payload).encode() if payload is not None else None
    req = urllib.request.Request(base+path, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=15) as response:
            code, raw = response.status, response.read().decode()
    except urllib.error.HTTPError as response:
        code, raw = response.code, response.read().decode()
    print(method, path, "->", code, flush=True)
    if code != expected:
        if "/auth/" not in path: print(raw, flush=True)
        raise AssertionError(f"Expected {expected}, got {code}")
    if raw and "/auth/" not in path: print(raw, flush=True)
    return json.loads(raw) if raw and code < 300 else None
spec = request("GET", "/swagger/v1/swagger.json")
auth = "/auth/login" in spec["paths"]
token = None
if auth:
    password=os.environ["BootstrapAdmin__Password"]
    admin=os.environ["BootstrapAdmin__Email"]
    email=f"viewer-{uuid.uuid4().hex[:10]}@example.test"
    request("POST","/auth/register",{"email":email,"password":password})
    viewer=request("POST","/auth/login?useCookies=false",{"email":email,"password":password})["accessToken"]
    token=request("POST","/auth/login?useCookies=false",{"email":admin,"password":password})["accessToken"]
    body={"title":"Test project","budget":1000,"address":"Test address","isCompleted":False}
    request("GET","/api/projects")
    request("POST","/api/projects",body,expected=401)
    request("POST","/api/projects",body,token=viewer,expected=403)
    users=request("GET","/api/users",token=token)
    uid=next(u["id"] for u in users if u["email"]==email)
    request("PUT",f"/api/users/{uid}/role",{"role":"Manager"},token,204)
    manager=request("POST","/auth/login?useCookies=false",{"email":email,"password":password})["accessToken"]
    item=request("POST","/api/projects",body,manager,201)
    request("DELETE","/api/projects/"+item["id"],token=manager,expected=403)
    request("DELETE","/api/projects/"+item["id"],token=token,expected=204)
for resource, body in [
    ("projects",{"title":"Test project","budget":1000,"address":"Test address","isCompleted":False}),
    ("builders",{"name":"Test builder","hourlyRate":100,"specialty":"Masonry","experienceYears":2,"safetyCertificate":"TEST"})]:
    p="/api/"+resource
    request("GET",p+"?page=1&amount=10")
    request("GET",p+"?page=0&amount=10",expected=400)
    obj=request("POST",p,body,token,201)
    url=p+"/"+obj["id"]
    request("GET",url)
    request("PUT",url,body,token,204)
    request("DELETE",url,token=token,expected=204)
    request("GET",url,expected=404)
print("All HTTP assertions passed.")
