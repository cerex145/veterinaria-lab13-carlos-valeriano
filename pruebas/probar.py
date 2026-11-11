import json, urllib.request, urllib.error, ssl, datetime, pathlib, html, sys
root=pathlib.Path(__file__).resolve().parents[1]; out=root/'pruebas';out.mkdir(exist_ok=True)
base='https://localhost:7243'; ctx=ssl._create_unverified_context(); results=[]
def req(label,method,path,status,data=None):
 body=json.dumps(data,ensure_ascii=False).encode('utf8') if data is not None else None
 request=urllib.request.Request(base+path,data=body,method=method,headers={'Content-Type':'application/json'} if body else {})
 started=datetime.datetime.now(datetime.timezone.utc)
 try: response=urllib.request.urlopen(request,context=ctx,timeout=25)
 except urllib.error.HTTPError as e: response=e
 raw=response.read().decode(); parsed=json.loads(raw) if raw else None
 item={'numero':len(results)+1,'prueba':label,'metodo':method,'ruta':path,'solicitud':data,'esperado':status,'estado':response.status,'cabeceras':dict(response.headers),'respuesta':parsed,'fechaUtc':started.isoformat(),'correcto':response.status==status}
 results.append(item); (out/'resultados.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf8')
 print(f'{len(results):02d} {method} {path} => {response.status} (esperado {status})',flush=True)
 assert response.status==status, item
 return parsed,item
req('Listado de propietarios','GET','/api/Propietarios',200)
req('Consulta de propietario','GET','/api/Propietarios/1',200)
req('Mascotas del propietario: ruta anidada','GET','/api/Propietarios/1/mascotas',200)
req('Listado de mascotas','GET','/api/Mascotas',200)
p,_=req('Consulta de mascota y nombre relacionado','GET','/api/Mascotas/1',200)
assert p['propietarioNombre']=='Ana Torres' and 'propietario' not in p
p,_=req('Filtro por nombre','GET','/api/Propietarios?nombre=Ana',200)
assert len(p)==1 and p[0]['nombre']=='Ana Torres'
p,_=req('Filtros por especie y propietario','GET','/api/Mascotas?especie=Perro&propietarioId=1',200)
assert len(p)==1 and p[0]['nombre']=='Luna'
req('Data Annotations en propietario','POST','/api/Propietarios',400,{'nombre':'A','dni':'12','telefono':'1'})
req('Data Annotations en mascota','POST','/api/Mascotas',400,{'nombre':'X','especie':'','edad':50,'propietarioId':0})
req('Propietario inexistente','GET','/api/Propietarios/999999',404)
req('Mascota inexistente','GET','/api/Mascotas/999999',404)
req('Regla: DNI duplicado','POST','/api/Propietarios',409,{'nombre':'Ana duplicada','dni':'12345678','telefono':'900000000'})
req('Regla: propietario con mascotas','DELETE','/api/Propietarios/1',409)
req('Regla: propietario de mascota inexistente','POST','/api/Mascotas',409,{'nombre':'Nube','especie':'Gato','edad':1,'propietarioId':999999})
owner,item=req('Crear propietario y Location','POST','/api/Propietarios',201,{'nombre':'Elena Prueba','dni':'45678912','telefono':'900111222'})
oid=owner['id']; location=next(v for k,v in item['cabeceras'].items() if k.lower()=='location')
assert location.endswith(f'/api/Propietarios/{oid}')
req('Verificar recurso de Location','GET',f'/api/Propietarios/{oid}',200)
req('Actualizar propietario','PUT',f'/api/Propietarios/{oid}',204,{'nombre':'Elena Actualizada','dni':'45678912','telefono':'900333444'})
pet,item=req('Crear mascota y Location','POST','/api/Mascotas',201,{'nombre':'Nube','especie':'Gato','edad':1,'propietarioId':oid})
pid=pet['id']; assert pet['propietarioNombre']=='Elena Actualizada'
assert next(v for k,v in item['cabeceras'].items() if k.lower()=='location').endswith(f'/api/Mascotas/{pid}')
req('Actualizar mascota y cambiar propietario','PUT',f'/api/Mascotas/{pid}',204,{'nombre':'Nube Actualizada','especie':'Gato','edad':2,'propietarioId':2})
pet,_=req('Verificar actualización y MapFrom','GET',f'/api/Mascotas/{pid}',200)
assert pet['nombre']=='Nube Actualizada' and pet['edad']==2 and pet['propietarioNombre']=='Luis Rojas'
req('Eliminar mascota','DELETE',f'/api/Mascotas/{pid}',204)
req('Eliminar propietario sin mascotas','DELETE',f'/api/Propietarios/{oid}',204)
req('PUT propietario inexistente','PUT','/api/Propietarios/999999',404,{'nombre':'Prueba Valida','dni':'45678912','telefono':'900333444'})
req('PUT mascota inexistente','PUT','/api/Mascotas/999999',404,{'nombre':'Prueba','especie':'Gato','edad':2,'propietarioId':2})
req('DELETE propietario inexistente','DELETE','/api/Propietarios/999999',404)
req('DELETE mascota inexistente','DELETE','/api/Mascotas/999999',404)
req('Ruta anidada de propietario inexistente','GET','/api/Propietarios/999999/mascotas',404)
swagger=json.load(urllib.request.urlopen(base+'/swagger/v1/swagger.json',context=ctx))
schemas=swagger['components']['schemas']; assert not {'Propietario','Mascota','VeterinariaContext'} & set(schemas)
assert sum(len([m for m in p if m in ['get','post','put','delete']]) for p in swagger['paths'].values())==11
(out/'openapi.json').write_text(json.dumps(swagger,ensure_ascii=False,indent=2),encoding='utf8')
http=['@baseUrl = https://localhost:7243','']
for r in results:
 http.extend([f"### {r['numero']:02d} {r['prueba']} (esperado {r['esperado']})",f"{r['metodo']} {{{{baseUrl}}}}{r['ruta']}"])
 if r['solicitud'] is not None: http.extend(['Content-Type: application/json','',json.dumps(r['solicitud'],ensure_ascii=False,indent=2)])
 http.append('')
(out/'endpoints.http').write_text('\n'.join(http),encoding='utf8')
print(f'RESULTADO: {len(results)}/{len(results)} pruebas correctas; 11 endpoints y Swagger con DTOs. IDs temporales eliminados: propietario {oid}, mascota {pid}.')
