# SolidarityGrid: Protocolo del Buen Samaritano 🛡️

Este proyecto implementa un sistema de procesamiento de pagos distribuido, resiliente y altamente disponible, diseñado para operar en un entorno de **Arquitectura Mesh (Malla)** sin depender de un Broker central (como RabbitMQ o Kafka).

##  Arquitectura del Sistema

El sistema consta de 3 nodos independientes ejecutándose en contenedores Docker. La coordinación se logra mediante un estado compartido en una base de datos **SQLite** optimizada para alta concurrencia.

### Componentes Clave:
- **Nodos (API REST):** Desarrollados en .NET 8, encargados de recibir y procesar pagos.
- **Vigilante (Background Service):** Un proceso en segundo plano que corre en cada nodo para monitorear la salud de sus pares.
- **Capa de Persistencia:** SQLite con modo **WAL (Write-Ahead Logging)** para permitir lecturas y escrituras simultáneas desde múltiples contenedores.



##  Cómo funciona el Protocolo

### 1. El Latido (Heartbeat)
Cada 3 segundos, cada nodo actualiza su marca de tiempo en la tabla `Estados`. Esto indica a los demás nodos que sigue operativo.

### 2. Detección de Fallos
Si un nodo no actualiza su estado por más de **10 segundos**, los nodos sobrevivientes lo declaran como "Caído".

### 3. Rescate de Carga (Ayuda Mutua)
Al detectar un nodo caído, el primer nodo disponible ejecuta un **Rescate Atómico**:
- Identifica los pagos que estaban en estado `Procesando` bajo la responsabilidad del nodo fallido.
- Cambia la propiedad de dichos pagos y los marca como `Completado`.
- Gracias al uso de `ExecuteUpdateAsync`, este proceso es **Idempotente** y seguro contra colisiones (evita que dos nodos rescaten lo mismo).

##  Despliegue con Docker

Para levantar la red de 3 nodos, simplemente ejecute:

```bash
docker-compose up --build