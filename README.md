# SaludIPS WhatsApp Assistant

Un asistente de WhatsApp para agendamiento de citas médicas construido con Clean Architecture en .NET 8.

## 🏗️ Arquitectura

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

- **Domain**: Entidades de negocio, value objects, enums y contratos de dominio
- **Application**: Casos de uso, comandos, consultas e interfaces de servicios
- **Infrastructure**: Implementaciones de servicios externos, repositorios y persistencia
- **WebApi**: Controladores API y configuración de la aplicación

## 🚀 Características

### Funcionalidades Principales
- ✅ Conversación natural por WhatsApp para agendar citas
- ✅ Integración con API externa de agendas médicas
- ✅ Manejo de estado de conversación por usuario
- ✅ Recordatorios automáticos (1 día y 1 hora antes)
- ✅ Procesamiento básico de lenguaje natural
- ✅ Arquitectura escalable y mantenible

### Flujo Conversacional
1. Usuario inicia conversación
2. Bot solicita especialidad médica
3. Sistema consulta citas disponibles en API externa
4. Usuario selecciona cita preferida
5. Confirmación y agendamiento
6. Programación de recordatorios automáticos

### Estados de Conversación
- `Initial`: Estado inicial
- `CollectingSpecialty`: Recopilando especialidad médica
- `ShowingAvailableSlots`: Mostrando citas disponibles
- `ConfirmingAppointment`: Confirmando cita seleccionada
- `AppointmentBooked`: Cita agendada exitosamente

## 🛠️ Tecnologías

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para persistencia
- **MediatR**: Patrón mediator para CQRS
- **FluentValidation**: Validación de comandos
- **Hangfire**: Jobs en segundo plano (para implementar)
- **SQL Server**: Base de datos
- **xUnit**: Testing framework
- **Docker**: Containerización

## 📋 Requisitos Previos

- .NET 8 SDK
- SQL Server o SQL Server Express
- Docker (opcional)
- Visual Studio 2022 o VS Code

## 🚀 Configuración e Instalación

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Juanprojas1982/SaludIPSAssistant.git
cd SaludIPSAssistant
```

### 2. Configurar Base de Datos
Actualizar la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SaludIPSAssistantDb;Trusted_Connection=true;"
  }
}
```

### 3. Ejecutar Migraciones
```bash
cd src/SaludIPSAssistant.WebApi
dotnet ef migrations add InitialCreate --project ../SaludIPSAssistant.Infrastructure
dotnet ef database update --project ../SaludIPSAssistant.Infrastructure
```

### 4. Configurar Servicios Externos

#### WhatsApp Business API
```json
{
  "WhatsApp": {
    "ApiUrl": "https://graph.facebook.com/v21.0/",
    "AccessToken": "tu_token_de_acceso",
    "VerifyToken": "tu_token_de_verificacion",
    "PhoneNumberId": "tu_id_de_numero_telefonico"
  }
}
```

#### API Externa de Agendas
```json
{
  "ExternalApi": {
    "BaseUrl": "https://tu-api-externa.com/api/",
    "ApiKey": "tu_clave_api"
  }
}
```

### 5. Ejecutar la Aplicación
```bash
dotnet run --project src/SaludIPSAssistant.WebApi
```

La aplicación estará disponible en `https://localhost:7049`

### 6. Docker (Alternativo)
```bash
docker-compose up -d
```

## 🧪 Ejecutar Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📱 Uso de la API

### Webhook de WhatsApp
```http
POST /api/webhook/whatsapp
Content-Type: application/json

{
  "from": "+573001234567",
  "text": "Hola, necesito una cita",
  "messageId": "msg_123",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Verificación de Webhook
```http
GET /api/webhook/whatsapp?hub.mode=subscribe&hub.challenge=123&hub.verify_token=tu_token
```

## 🔧 Desarrollo

### Estructura del Proyecto
```
SaludIPSAssistant/
├── src/
│   ├── SaludIPSAssistant.Domain/      # Entidades y lógica de negocio
│   ├── SaludIPSAssistant.Application/ # Casos de uso y contratos
│   ├── SaludIPSAssistant.Infrastructure/ # Servicios externos y datos
│   └── SaludIPSAssistant.WebApi/      # API controllers y configuración
├── tests/
│   ├── SaludIPSAssistant.UnitTests/   # Pruebas unitarias
│   └── SaludIPSAssistant.IntegrationTests/ # Pruebas de integración
├── Dockerfile
├── docker-compose.yml
└── README.md
```

### Agregar Nueva Especialidad
1. Actualizar enum si es necesario
2. Modificar keywords en `SimpleNlpService`
3. Agregar mapeo en `MockAgendaProvider`

### Extender Conversación
1. Agregar nuevo estado en `ConversationState`
2. Implementar handler en `ProcessIncomingMessageHandler`
3. Crear tests correspondientes

## 🔒 Seguridad

- Tokens JWT para endpoints administrativos (pendiente implementar)
- Validación de webhook de WhatsApp
- Encriptación de datos sensibles (pendiente implementar)
- Control de idempotencia para evitar duplicados

## 📈 Monitoreo y Logging

- Logging estructurado con Serilog (pendiente implementar)
- Métricas básicas de rendimiento
- Health checks para servicios externos

## 🚀 Despliegue

### CI/CD con GitHub Actions
El proyecto incluye pipeline automatizado que:
- Ejecuta pruebas
- Analiza calidad de código
- Construye imagen Docker
- Ejecuta escaneo de seguridad

### Variables de Entorno para Producción
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="tu_cadena_de_conexion"
WhatsApp__AccessToken="tu_token_de_whatsapp"
ExternalApi__ApiKey="tu_clave_api_externa"
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crear feature branch (`git checkout -b feature/nueva-caracteristica`)
3. Commit cambios (`git commit -am 'Agregar nueva característica'`)
4. Push al branch (`git push origin feature/nueva-caracteristica`)
5. Crear Pull Request

## 📝 Próximas Mejoras

- [ ] Implementar Hangfire para jobs programados
- [ ] Integrar servicio NLP más avanzado (Rasa/LUIS)
- [ ] Agregar autenticación JWT
- [ ] Implementar cache con Redis
- [ ] Mejorar manejo de errores y resiliencia
- [ ] Agregar más pruebas de integración
- [ ] Implementar métricas y monitoring

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 📞 Contacto

- **Autor**: Juan
- **GitHub**: [@Juanprojas1982](https://github.com/Juanprojas1982)

---

⭐ ¡No olvides dar una estrella al proyecto si te fue útil!