# 🖥️ Backend - Aplicación de Gestión de Gastos

Este es el backend de la aplicación de gestión económica personal/profesional, diseñado para proporcionar los servicios necesarios para almacenar, consultar, modificar y eliminar información relacionada con **ingresos, gastos, cuentas, personas, clientes, proveedores y más**.

---

## 🌐 Complemento del Frontend

Este backend da soporte a la interfaz alojada en:  
👉 [https://ahorroland.sergioizq.es](https://ahorroland.sergioizq.es)

---

## 🚀 Funcionalidades

El backend expone una API RESTful que cubre los siguientes módulos:

### 📥 Ingresos
- Crear, consultar, actualizar y eliminar ingresos.
- Filtro por fecha, categoría, y palabras clave.

### 💸 Gastos
- Registrar y gestionar gastos.
- Soporte para clasificación por categoría y concepto.

### 🗂️ Categorías
- Módulo CRUD completo para categorías de ingresos y gastos.

### 🧾 Conceptos
- Administración de conceptos personalizados para clasificar movimientos.

### 📊 Resumen Financiero
- Endpoint para obtener resumen de ingresos, gastos y beneficio neto en un rango de fechas.

### 🔄 Traspasos
- Registrar y consultar transferencias de dinero entre cuentas.

### 🏦 Cuentas
- Crear, actualizar y eliminar cuentas bancarias o de caja.
- Consultar todas las cuentas registradas.

### 💳 Formas de Pago
- Añadir y gestionar métodos de pago (efectivo, tarjeta, transferencia, etc.).

### 👤 Personas
- Registrar personas asociadas con tu actividad económica (clientes, proveedores).
- CRUD para gestionar esta información.

### 🧑‍💼 Clientes
- Registrar y gestionar clientes con sus datos detallados.

### 🏭 Proveedores
- Gestión de proveedores con opciones de actualización y eliminación.

---

## 🔧 Tecnologías Utilizadas

- **Node.js** (para el servidor)
- **Express.js** (para la API RESTful)
- **MongoDB** (base de datos NoSQL)
- **JWT** (para autenticación y autorización)
- **Mongoose** (para interacción con MongoDB)
- **Swagger** (para documentación de la API)

---

## 📌 Notas

- Este backend requiere de un **frontend** para la interacción del usuario. El frontend está disponible en:  
  👉 [https://appg.sergioizq.es](https://appg.sergioizq.es)
- La **autenticación** se maneja a través de JWT, por lo que es necesario un token válido para acceder a los endpoints protegidos.
- **CORS** está habilitado para permitir que el frontend y el backend se comuniquen sin problemas de restricciones de origen cruzado.

---

## 🛠️ Instalación y Ejecución

### 1. Clona el repositorio:
```bash
git clone https://github.com/tu-usuario/gestion-gastos-backend.git
