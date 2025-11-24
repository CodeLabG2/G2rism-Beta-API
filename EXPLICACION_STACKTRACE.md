# 📋 Explicación del StackTrace

Este documento explica qué es un StackTrace, cómo leerlo y qué significan tanto el **stackTrace antiguo** como el **nuevo stackTrace mejorado** que implementamos.

---

## 🤔 ¿Qué es un StackTrace?

Un **StackTrace** (traza de pila) es un **registro detallado del camino que recorrió tu código** desde que comenzó hasta que ocurrió el error. Es como una "grabación" paso a paso de qué métodos se llamaron y en qué orden.

### Analogía Simple:

Imagina que tu código es una serie de habitaciones en una casa:

1. Entras por la **Puerta Principal** (Middleware)
2. Pasas por el **Pasillo** (ASP.NET Core Pipeline)
3. Entras a la **Sala de Control** (Controlador - `UsuariosController`)
4. Bajas a la **Bodega** (Servicio - `UsuarioService`)
5. Abres una **Caja** (Repositorio - `UsuarioRolRepository`)
6. **💥 Encuentra un problema** (Error)

El StackTrace te muestra exactamente **por cuáles habitaciones pasaste** para llegar al error.

---

## 📜 StackTrace ANTIGUO (Sin Formatear)

Aquí está el stackTrace completo que viste originalmente:

```
at G2rismBeta.API.Services.UsuarioService.AsignarRolesAsync(Int32 idUsuario, List`1 rolesIds, Nullable`1 asignadoPor) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Services\UsuarioService.cs:line 329
at G2rismBeta.API.Controllers.UsuariosController.AsignarRoles(Int32 id, AsignarRolesMultiplesDto dto) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Controllers\UsuariosController.cs:line 550
at lambda_method255(Closure, Object)
at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.AwaitableObjectResultExecutor.Execute(ActionContext actionContext, IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Awaited|12_0(ControllerActionInvoker invoker, ValueTask`1 actionResultValueTask)
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
at G2rismBeta.API.Middleware.GlobalExceptionHandlerMiddleware.InvokeAsync(HttpContext context) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Middleware\GlobalExceptionHandlerMiddleware.cs:line 34
```

---

## 🔍 Desglose del StackTrace ANTIGUO

Voy a explicarte **línea por línea** qué significa cada parte:

### **Línea 1: EL ERROR OCURRIÓ AQUÍ** 🔴

```
at G2rismBeta.API.Services.UsuarioService.AsignarRolesAsync(Int32 idUsuario, List`1 rolesIds, Nullable`1 asignadoPor)
in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Services\UsuarioService.cs:line 329
```

**Qué significa:**
- `at G2rismBeta.API.Services.UsuarioService.AsignarRolesAsync` → En el método `AsignarRolesAsync` del servicio `UsuarioService`
- `(Int32 idUsuario, List`1 rolesIds, Nullable`1 asignadoPor)` → Con estos parámetros
- `in C:\...\UsuarioService.cs` → Ubicado en este archivo
- `:line 329` → **Línea 329 exactamente** donde ocurrió el error

**Este es el punto exacto donde tu código lanzó la excepción** (`throw new InvalidOperationException(...)`).

---

### **Línea 2: QUIÉN LLAMÓ AL MÉTODO CON ERROR** 🔴

```
at G2rismBeta.API.Controllers.UsuariosController.AsignarRoles(Int32 id, AsignarRolesMultiplesDto dto)
in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Controllers\UsuariosController.cs:line 550
```

**Qué significa:**
- El controlador `UsuariosController` llamó al método anterior
- En el método `AsignarRoles` (tu endpoint de la API)
- En la **línea 550** del archivo `UsuariosController.cs`

**Esta es la entrada del usuario a tu código** (cuando hizo el request HTTP).

---

### **Línea 3-15: FRAMEWORK INTERNO (ASP.NET Core)** ⚪

```
at lambda_method255(Closure, Object)
at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor...
at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker...
...
at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware...
```

**Qué significa:**
Todas estas líneas son **código interno del framework ASP.NET Core y Swagger**. Muestran cómo el framework:
1. Recibió la petición HTTP
2. Pasó por los middlewares (Swagger, Autorización, etc.)
3. Ejecutó el controlador
4. Invocó el método de acción
5. Manejó la excepción

**No son parte de tu código**, son el "plumbing" interno de ASP.NET Core.

---

### **Línea Final: MIDDLEWARE QUE CAPTURÓ EL ERROR** 🛡️

```
at G2rismBeta.API.Middleware.GlobalExceptionHandlerMiddleware.InvokeAsync(HttpContext context)
in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Middleware\GlobalExceptionHandlerMiddleware.cs:line 34
```

**Qué significa:**
- Tu middleware `GlobalExceptionHandlerMiddleware` capturó y procesó la excepción
- En la **línea 34** (el `await _next(context);`)

**Este es el punto de entrada de toda la petición HTTP**.

---

## 🎯 StackTrace NUEVO (Mejorado y Formateado)

Ahora verás un stackTrace mucho más limpio y fácil de entender:

```
📋 TRAZA DE EJECUCIÓN DEL ERROR:
🔴 = Tu código (G2rismBeta.API)
⚪ = Framework (ASP.NET Core / EF Core)

  [1] 🔴 at G2rismBeta.API.Services.UsuarioService.AsignarRolesAsync(Int32 idUsuario, List`1 rolesIds, Nullable`1 asignadoPor) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Services\UsuarioService.cs:line 329
  [2] 🔴 at G2rismBeta.API.Controllers.UsuariosController.AsignarRoles(Int32 id, AsignarRolesMultiplesDto dto) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Controllers\UsuariosController.cs:line 550
  [3] ⚪ ASP.NET Core: Ejecutando método del controlador
  [4] ⚪ ASP.NET Core: Ejecutando acción del controlador
  [5] ⚪ ASP.NET Core: Verificando autorización
  [6] ⚪ Swagger: Procesando solicitud de documentación
  [7] 🔴 at G2rismBeta.API.Middleware.GlobalExceptionHandlerMiddleware.InvokeAsync(HttpContext context) in C:\Dev 💻\CodeLabG2\Beta Projects\1st Project\App\G2rismBeta.API\Middleware\GlobalExceptionHandlerMiddleware.cs:line 34
```

---

## 🔍 Desglose del StackTrace NUEVO

### **¿Qué cambió?**

1. **Header explicativo** 📋:
   - Te dice qué significan los colores
   - Diferencia entre **tu código** (🔴) y el **framework** (⚪)

2. **Numeración** [1], [2], [3]...:
   - Cada paso está numerado para seguir el flujo fácilmente
   - Puedes decir "el error está en el paso [1]"

3. **Filtrado inteligente** 🧠:
   - Las líneas del framework que no son relevantes **se resumen en mensajes simples**
   - En lugar de `Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.AwaitableObjectResultExecutor.Execute(...)`, ves:
     ```
     ⚪ ASP.NET Core: Ejecutando método del controlador
     ```

4. **Resaltado de tu código** 🔴:
   - Tus archivos (`G2rismBeta.API`) están marcados con 🔴
   - El framework está marcado con ⚪
   - **Encuentras rápidamente dónde está tu error**

---

## 📊 Comparación Visual

### StackTrace Antiguo (17 líneas técnicas):
```
❌ Confuso
❌ Mucha información irrelevante
❌ Difícil de identificar tu código
✅ Completo y detallado
```

### StackTrace Nuevo (7 líneas relevantes):
```
✅ Claro y conciso
✅ Solo información importante
✅ Tu código resaltado en rojo
✅ Framework resumido
```

---

## 🛠️ Cómo Usar el StackTrace para Debuggear

### **Paso 1: Lee de arriba hacia abajo** ⬇️

El stackTrace se lee **de arriba hacia abajo** como una lista de pasos:

```
[1] 🔴 UsuarioService.AsignarRolesAsync:329     ← ¡AQUÍ ESTÁ EL ERROR!
[2] 🔴 UsuariosController.AsignarRoles:550      ← Quién lo llamó
[3] ⚪ ASP.NET Core pipeline                    ← Cómo llegó
```

### **Paso 2: Identifica las líneas rojas (🔴)** 🎯

Estas son **TUS archivos**, donde puedes hacer cambios:
- `UsuarioService.cs:329` → Ve a la línea 329 de ese archivo
- `UsuariosController.cs:550` → Ve a la línea 550 de ese archivo

### **Paso 3: Ignora las líneas blancas (⚪)** ⏭️

Estas son del framework (ASP.NET Core, Entity Framework). No necesitas preocuparte por ellas a menos que haya un bug del framework (muy raro).

### **Paso 4: Revisa el código en la primera línea roja** 🔍

En el ejemplo:
```
[1] 🔴 UsuarioService.cs:line 329
```

Vas al archivo `UsuarioService.cs`, línea 329, y encuentras:
```csharp
throw new InvalidOperationException(errorSuperAdmin!);
```

**¡Ahí está el error!** Ahora sabes exactamente dónde y por qué falló.

---

## 🎓 Caso Real: Entendiendo Tu Error

Tomemos el error que encontraste:

### **Error:**
```
Ya existe un Súper Administrador en el sistema (Usuario: Villa, ID: 6).
Solo puede haber un Súper Administrador a la vez.
```

### **StackTrace Nuevo:**
```
[1] 🔴 UsuarioService.AsignarRolesAsync:329
[2] 🔴 UsuariosController.AsignarRoles:550
```

### **¿Qué pasó?**

1. **Usuario hizo request**: `POST /api/usuarios/7/asignar-roles` con `{"rolesIds": [1]}`
2. **Entró al controlador**: Línea 550 de `UsuariosController.cs`
   ```csharp
   await _usuarioService.AsignarRolesAsync(id, dto.RolesIds);
   ```
3. **Fue al servicio**: Línea 329 de `UsuarioService.cs`
   ```csharp
   throw new InvalidOperationException(
       "Ya existe un Súper Administrador en el sistema (Usuario: Villa, ID: 6)..."
   );
   ```
4. **Validación falló**: Ya existe un Súper Admin (Villa con ID 6)
5. **Se lanzó el error**: `InvalidOperationException`
6. **Middleware lo capturó**: Devolvió respuesta JSON con código 400

---

## ✅ Resumen

| Característica | StackTrace Antiguo | StackTrace Nuevo |
|----------------|-------------------|------------------|
| **Longitud** | ~17 líneas | ~7 líneas |
| **Claridad** | Técnico y verboso | Simple y visual |
| **Identificación de tu código** | Manual | Automática (🔴) |
| **Framework** | Todas las líneas | Resumido (⚪) |
| **Legibilidad** | Baja | Alta |
| **Útil para debugging** | Sí, pero difícil | Sí, muy fácil |

---

## 🚀 Beneficios del StackTrace Mejorado

1. ✅ **Encuentras errores más rápido**: Las líneas rojas te dicen exactamente dónde mirar
2. ✅ **Menos información innecesaria**: No te distraes con código del framework
3. ✅ **Mejor experiencia de desarrollo**: Debugging más rápido y eficiente
4. ✅ **Fácil de compartir**: Puedes copiar solo las líneas relevantes al reportar bugs
5. ✅ **Aprendizaje más rápido**: Entiendes el flujo de tu aplicación visualmente

---

## 💡 Consejos Finales

1. **En producción**: El stackTrace NO se muestra al usuario (solo en desarrollo)
2. **Para logs**: Siempre guarda el stackTrace completo en tus logs
3. **Para usuarios**: Solo muestra el mensaje de error amigable
4. **Para debugging**: Usa el stackTrace mejorado para identificar el problema rápido

---

## 📝 Ejemplo Práctico Completo

### Request del Usuario:
```http
POST /api/usuarios/7/asignar-roles
{
  "rolesIds": [1]
}
```

### Respuesta con StackTrace Nuevo:
```json
{
  "success": false,
  "message": "Ya existe un Súper Administrador en el sistema (Usuario: Villa, ID: 6)...",
  "statusCode": 400,
  "errorCode": "InvalidOperationException",
  "stackTrace": "📋 TRAZA DE EJECUCIÓN DEL ERROR:\n🔴 = Tu código\n⚪ = Framework\n\n[1] 🔴 UsuarioService.AsignarRolesAsync:329\n[2] 🔴 UsuariosController.AsignarRoles:550\n[3] ⚪ ASP.NET Core pipeline",
  "timestamp": "2025-11-23T21:45:17Z"
}
```

### ¿Qué hacer?
1. Lee el mensaje: "Ya existe un Súper Administrador..."
2. Ve al stackTrace: línea [1] `UsuarioService:329`
3. Abre el archivo y revisa la lógica
4. Confirma que la validación funciona correctamente ✅

---

**Generado automáticamente - G2rism Beta API**
**CodeLabG2 - Sistema de Turismo**
