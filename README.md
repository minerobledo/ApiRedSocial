# SocialNetwork API Core

Backend robusto para una red social desarrollado con **.NET 8**. Este proyecto fue creado originalmente de forma freelance pero nunca llego completamente a produccion y ahora se presenta como parte de mi portfolio técnico.

## 🚀 Stack Tecnológico

*   **Framework:** .NET 8.0 (Core)
*   **Autenticación:** JWT (JSON Web Tokens) con implementación manual[cite: 1]
*   **Tiempo Real:** Chat mediante WebSockets para mensajería instantánea[cite: 1]
*   **Cloud & Storage:** Integración con Firebase para gestión de archivos y datos[cite: 1]
*   **Seguridad:** Configuración de políticas de CORS para consumo desde el frontend[cite: 1]

## 🛠️ Características Principales

*   **Mensajería Instantánea:** Comunicación bidireccional en tiempo real mediante WebSockets[cite: 1]
*   **Seguridad personalizada:** Manejo de autorización y autenticación sin librerías de caja negra, permitiendo control total del ciclo de vida del token[cite: 1]
*   **Gestión Multimedia:** Subida y administración de archivos a través de Firebase SDK[cite: 1]
*   **Arquitectura Limpia:** Organización de carpetas orientada a mantenibilidad y escalabilidad.

## ⚙️ Configuración del Entorno

este proyecto esta pensado para trabajarce despde railwey o ser debujeado desde la computadora del desarollador y en ningun momento llego a produccion.
crear un archivo .env con lo sigiente o poner laas mimsas variables en railwey:

//------------------------------------------------------------------------------//
# Configuración del Servidor
PORT=5000
Cors_AllowedOrigins_="http://localhost:3000"

# Autenticación JWT
JWT_SECRET="esta_clave_es_un_chiste_total_xd"
JWT_ISSUER="MiAppSocial"
JWT_AUDIENCE="UsuariosApp"

# Encriptación Interna (AuthService)
AUTH_KEY="una_clave_de_32_caracteres_exacto"
AUTH_VECTOR="clave_de_16_chars"

# Firebase Service Account (Muy importante)
type="service_account"
project_id="tu-proyecto-id"
private_key_id="tu_id_privado"
private_key="-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n"
client_email="firebase-adminsdk@tu-proyecto.iam.gserviceaccount.com"
FIREBASE_CLIENT_ID="123456789"
auth_uri="https://accounts.google.com/o/oauth2/auth"
token_uri="https://oauth2.googleapis.com/token"
auth_provider_x509_cert_url="https://www.googleapis.com/oauth2/v1/certs"
client_x509_cert_url="https://www.googleapis.com/robot/v1/metadata/x509/..."
universe_domain="googleapis.com"

# Servicios de Email (AWS SES / Otros)
EMAIL_SENDER_API="https://tu-api-email.com"
SES_SENDER_EMAIL="tu-email@verificado.com"
AWS_ACCESS_KEY_ID="TU_AWS_KEY"
AWS_SECRET_ACCESS_KEY="TU_AWS_SECRET"
//------------------------------------------------------------------------------//