# Decisiones técnicas

## Manejo de fecha y hora

- Todas las fechas que llegan a la API se reciben como string y son validadas para que incluyan su desplazamiento(offset).
- Se usa DateTimeOffset después de validar el formato para procesar dentro del servidor.
- Se persiste en base de datos en formato UTC como estándar que puede ser formateado a la hora local del puerto.
- La hora operativa se evalúa usando la hora local del puerto de zarpe.

> La razón de recibir como string y no como DateTimeOffset es porque uno de los requerimientos exige que toda fecha recibida por la API venga con el offset explícito. Si lo recibo directamente como DateTimeOffset .Net podría interpretar que es una fecha válida y  completar el desplazamiento con la información del servidor y dejar pasar una fecha que no debería haber sido aceptada. Al recibirlo como string puedo aplicar validaciones para comprobar que ha incluido el desplazamiento. 
>Un string no permite realizar operaciones de reglas de negocio por lo que una vez validado en formato se convierte en DateTimeOffset y finalmente se persiste como UTC estandarizado que puede ser convertido a la hora local del puerto para evitar ambigüedades sin importar desde donde se  consulte. 

## Validaciones y reglas de negocio
- FluentValidation se utiliza para validar el formato de los datos de entrada.
- Las entidades Segment e Itinerary son entidades ricas que tienen métodos para cumplir con reglas de negocio propias de la entidad .
- El servicio gestiona las reglas que necesitan datos externos a la entidad.

## Manejo de errores
- Estandaricé el manejo de errores mediante pattern Result para gestionar errores de dominio esperados y clasificarlos por tipo. Posteriormente en base al tipo de error retorno el estado HTTP apropiado para el cliente.

