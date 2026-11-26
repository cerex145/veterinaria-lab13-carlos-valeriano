import os, json, urllib.request, urllib.error, datetime, pathlib, secrets, sys
base=os.environ.get('GATEWAY_URL','http://localhost:5100').rstrip('/')
vacunas=os.environ.get('VACUNAS_URL','http://localhost:5056').rstrip('/')
dest=pathlib.Path(os.environ.get('RESULT_DIR','pruebas/s15/local'))
dest.mkdir(parents=True,exist_ok=True)
results=[]
tokens={}
def req(label,method,path,status,data=None,role=None,direct=False):
 url=(vacunas if direct else base)+path
 headers={'Content-Type':'application/json'}
 if role: headers['Authorization']='Bearer '+tokens[role]
 payload=json.dumps(data).encode() if data is not None else None
 started=datetime.datetime.now(datetime.timezone.utc).isoformat()
 try: response=urllib.request.urlopen(urllib.request.Request(url,data=payload,headers=headers,method=method),timeout=120)
 except urllib.error.HTTPError as error: response=error
 raw=response.read().decode()
 try: parsed=json.loads(raw) if raw else None
 except ValueError: parsed=raw
 public_data={k:('[oculta]' if k=='clave' else v) for k,v in data.items()} if data else None
 public_response=dict(parsed,token='[JWT omitido]') if isinstance(parsed,dict) and 'token' in parsed else parsed
 results.append({'numero':len(results)+1,'prueba':label,'fechaUtc':started,'metodo':method,'url':url,'rol':role or 'Sin token','solicitud':public_data,'esperado':status,'estado':response.status,'respuesta':public_response,'correcto':response.status==status})
 (dest/'resultados.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
 print(f'{len(results):02d} {label}: {response.status} / {status}',flush=True)
 assert response.status==status,(label,response.status,raw)
 return parsed
for role in ['Recepcionista','Veterinario']:
 p=req('Login de '+role,'POST','/gateway/auth/login',200,{'correo':role.lower()+'@veterinaria.test','clave':os.environ[role.upper()+'_PASSWORD']})
 tokens[role]=p['token']
req('Catalogo por gateway sin JWT','GET','/gateway/vacunas',401)
req('Servicio 2 directo sin JWT','GET','/api/vacunas',401,direct=True)
req('Escritura sin JWT','POST','/gateway/propietarios',401,{'nombre':'Prueba','dni':'92345678','telefono':'900000000'})
req('Perfil sin JWT','GET','/gateway/auth/perfil',401)
req('GET de la S14 conserva autenticacion','GET','/gateway/propietarios',401)
req('Credenciales invalidas','POST','/gateway/auth/login',401,{'correo':'noexiste@veterinaria.test','clave':'ClaveIncorrecta123'})
tokens['Invalido']=tokens['Veterinario'][:-8]+'invalidx'
req('Firma invalida en gateway','GET','/gateway/vacunas',401,role='Invalido')
req('Firma invalida en servicio 2','GET','/api/vacunas',401,role='Invalido',direct=True)
req('Lista de vacunas Recepcionista','GET','/gateway/vacunas',200,role='Recepcionista')
req('Vacuna por id Veterinario','GET','/gateway/vacunas/1',200,role='Veterinario')
req('Vacuna inexistente','GET','/gateway/vacunas/999999',404,role='Veterinario')
req('Perfil desde claims','GET','/gateway/auth/perfil',200,role='Veterinario')
req('Filtro propietarios','GET','/gateway/propietarios?nombre=Ana',200,role='Recepcionista')
req('Filtro mascotas','GET','/gateway/mascotas?especie=Perro&propietarioId=1',200,role='Veterinario')
req('Rol sin permiso de alta','POST','/gateway/propietarios',403,{'nombre':'Prueba','dni':'92345678','telefono':'900000000'},role='Veterinario')
dni='9'+str(secrets.randbelow(10000000)).zfill(7)
owner=req('Crear propietario','POST','/gateway/propietarios',201,{'nombre':'Prueba Semana Quince','dni':dni,'telefono':'900111222'},role='Recepcionista')
oid=owner['id']
req('Propietario por id','GET',f'/gateway/propietarios/{oid}',200,role='Veterinario')
req('Actualizar propietario','PUT',f'/gateway/propietarios/{oid}',204,{'nombre':'Prueba Actualizada','dni':dni,'telefono':'900111222'},role='Recepcionista')
pet=req('Crear mascota','POST','/gateway/mascotas',201,{'nombre':'Nube','especie':'Gato','edad':2,'propietarioId':oid},role='Recepcionista')
pid=pet['id']
req('Ruta anidada con comodin','GET',f'/gateway/propietarios/{oid}/mascotas',200,role='Veterinario')
req('Recepcionista no modifica mascota','PUT',f'/gateway/mascotas/{pid}',403,{'nombre':'Nube','especie':'Gato','edad':3,'propietarioId':oid},role='Recepcionista')
req('Veterinario actualiza mascota','PUT',f'/gateway/mascotas/{pid}',204,{'nombre':'Nube','especie':'Gato','edad':3,'propietarioId':oid},role='Veterinario')
req('Regla propietario con mascota','DELETE',f'/gateway/propietarios/{oid}',409,role='Recepcionista')
req('Eliminar mascota','DELETE',f'/gateway/mascotas/{pid}',204,role='Veterinario')
req('Mascota eliminada','GET',f'/gateway/mascotas/{pid}',404,role='Veterinario')
req('Eliminar propietario','DELETE',f'/gateway/propietarios/{oid}',204,role='Recepcionista')
mail='prueba.'+secrets.token_hex(5)+'@veterinaria.test'
req('Registro publico impone rol','POST','/gateway/auth/registro',201,{'correo':mail,'clave':secrets.token_urlsafe(16),'rol':'Veterinario'})
req('Correo repetido','POST','/gateway/auth/registro',409,{'correo':mail.upper(),'clave':secrets.token_urlsafe(16)})
req('Validacion de datos','POST','/gateway/auth/registro',400,{'correo':'invalido','clave':'1'})
req('Ruta no configurada','GET','/gateway/desconocida',404)
print(f'{len(results)}/{len(results)} pruebas correctas. Detener el servicio 2 para comprobar el 502.',flush=True)
