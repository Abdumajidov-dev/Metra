# METRA v3.0 - To'liq Loyiha Tavsifi

## 📋 LOYIHA HAQIDA

**Nomi:** Metra v3.0 - Ijaraga Boshqaruv Tizimi
**Turi:** Desktop Application (WPF, Windows)
**Versiya:** 3.0 (Metra v2.0 dan to'liq qayta yozilgan)
**Til:** O'zbek tili
**Maqsad:** Qurilma va materiallarni ijaraga berish biznesini boshqarish

### Kim Ishlatadi:
- ✅ **Faqat ADMINLAR** (internal users)
- ❌ Oddiy foydalanuvchilar yo'q
- ❌ Public access yo'q
- Admin login qiladi va tizimni to'liq boshqaradi

---

## 🏗️ TEXNOLOGIYALAR

### Frontend (Desktop):
- **Framework:** WPF (.NET 8.0)
- **Pattern:** MVVM (Model-View-ViewModel)
- **DI Container:** Microsoft.Extensions.DependencyInjection
- **UI Libraries:**
  - MaterialDesignThemes 5.1.0
  - Syncfusion WPF 27.1.55
- **MVVM Helper:** CommunityToolkit.Mvvm
- **Notifications:** Notification.Wpf

### Backend Connection:
- **API Base URL:** `http://app.metra-rent.uz/api`
- **Protocol:** REST API (HTTP/HTTPS)
- **Auth:** Bearer Token (JWT)
- **HTTP Client:** HttpClientFactory
- **Serialization:** Newtonsoft.Json

### Infrastructure:
- **Logging:** Serilog (File + Console)
- **PDF Generation:** FreeSpire.PDF
- **QR Codes:** QRCoder
- **Settings Storage:** Local encrypted file

---

## 🏛️ ARXITEKTURA

### Clean Architecture (4 Layer):

```
┌─────────────────────────────────────┐
│   Metra.Desktop (Presentation)      │
│   - Views (XAML)                    │
│   - ViewModels (presentation logic) │
│   - Converters, Styles              │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Metra.Application (Business)      │
│   - Services (interfaces + impl)    │
│   - DTOs (Request/Response)         │
│   - Validators                      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Metra.Domain (Core)               │
│   - Entities (domain models)        │
│   - Enums                           │
│   - Exceptions                      │
└─────────────────────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Metra.Infrastructure (External)   │
│   - API clients                     │
│   - Local storage                   │
│   - Logging config                  │
└─────────────────────────────────────┘
```

---

## 🖥️ MAINWINDOW TUZILISHI

### Layout:

```
┌─────────────────────────────────────────────────────────┐
│  Top AppBar (60px) - Purple #4447E2                     │
│  [METRA v3.0]                    [Settings] [Logout]    │
├──────────────┬──────────────────────────────────────────┤
│              │                                          │
│  Sidebar     │  Main Content Area                      │
│  (250px)     │  <ContentControl x:Name="ContentArea">  │
│              │                                          │
│  Navigation  │  - Welcome Screen (default)             │
│  Menu:       │  - Filiallar Page                       │
│              │  - Mijozlar Page                        │
│  📊 Bosh     │  - Shartnomalar Page                    │
│  📁 Ma'lumot │  - va hokazo...                         │
│  📄 Hujjat   │                                          │
│  📊 Hisobot  │                                          │
│  📦 Ombor    │                                          │
│  👥 Xodimlar │                                          │
│              │                                          │
└──────────────┴──────────────────────────────────────────┘
```

### Navigation Mexanizmi:
1. Sidebar da ListBoxItem bosiladi
2. MainWindow.xaml.cs da SelectionChanged event
3. Switch-case orqali page aniqlanadi
4. ServiceProvider dan page oladi (DI)
5. ContentArea.Content ga yuklaydi
6. Page ViewModelni avtomatik initialize qiladi

---

## 📑 BARCHA MODULLAR (PAGES)

### 1️⃣ Bosh Sahifa (Welcome Screen)
- **Vazifa:** Boshlang'ich ekran
- **Ko'rinish:** Logo, sarlavha, versiya
- **Ma'lumot:** Static, API yo'q

---

### 2️⃣ Ma'lumotlar Moduli

#### 📍 Filiallar (Branches)
**API Endpoint:**
- GET `/branches` - ro'yxat olish
- POST `/branches` - qidiruv (body: `{branch_name: "..."}`)
- GET `/branches/{id}` - bitta filial
- POST `/branch` - yangi yaratish
- PUT `/branch/{id}` - tahrirlash
- DELETE `/branch/delete/{id}` - o'chirish

**Ma'lumotlar:**
- id, name, description, type (main/branch/warehouse)
- responsible_worker, created_at, updated_at

**Funksiyalar:**
- Filiallar ro'yxati (DataGrid)
- Qidiruv (debounce 500ms)
- Yangi filial qo'shish (dialog)
- Tahrirlash (dialog)
- O'chirish (confirmation)

---

#### 👥 Mijozlar (Customers/Clients)
**API Endpoint:**
- POST `/customers` - ro'yxat + qidiruv (pagination)
- GET `/customers/{id}` - bitta mijoz
- POST `/customer` - yangi yaratish
- PUT `/customer/{id}` - tahrirlash
- DELETE `/customer/{id}` - o'chirish

**Ma'lumotlar:**
- id, full_name, phone, passport_serial, address
- birth_date, additional_phone, created_at

**Funksiyalar:**
- Mijozlar ro'yxati (pagination)
- Qidiruv (ism, telefon, passport)
- CRUD operatsiyalari
- Mijoz tarixi ko'rish

---

#### 🔧 Materiallar (Equipment/Materials)
**API Endpoint:**
- POST `/materials` - ro'yxat + filter
- GET `/materials/{id}` - bitta material
- POST `/material` - yangi qo'shish
- PUT `/material/{id}` - tahrirlash
- DELETE `/material/{id}` - o'chirish

**Ma'lumotlar:**
- id, name, category_id, inner_category_id
- price_per_day, quantity, available_quantity
- barcode, qr_code, image, status

**Funksiyalar:**
- Katalog ko'rish
- Kategoriyalar bo'yicha filter
- Narx belgilash
- Rasim yuklash
- QR kod yaratish
- Mavjudlik tracking

---

#### 📂 Kategoriyalar
**API Endpoints:**
- POST `/categories` - asosiy kategoriyalar
- POST `/inner-categories` - ichki kategoriyalar

**Funksiyalar:**
- Kategoriya daraxtini ko'rish
- CRUD operatsiyalari

---

### 3️⃣ Hujjatlar Moduli

#### 📄 Shartnomalar (Contracts)
**API Endpoint:**
- POST `/contracts` - ro'yxat + filter
- GET `/contracts/{id}` - bitta shartnoma
- POST `/contract` - yangi yaratish
- PUT `/contract/{id}` - tahrirlash
- PUT `/contract/{id}/status` - status o'zgartirish
- DELETE `/contract/{id}` - bekor qilish

**Ma'lumotlar:**
- id, contract_number, customer_id, branch_id
- start_date, end_date, total_amount, paid_amount
- status (active/completed/cancelled), materials[]
- payment_schedule[], created_at

**Funksiyalar:**
- Shartnomalar ro'yxati
- Yangi shartnoma tuzish
- Materiallar tanlash
- To'lov jadvali
- Status tracking (active → completed)
- PDF export
- Bekor qilish sabablari

---

#### 🧾 Fakturalar (Invoices)
**API Endpoint:**
- POST `/invoices` - ro'yxat
- GET `/invoices/{id}` - bitta faktura
- POST `/invoice` - yaratish
- DELETE `/invoice/{id}` - o'chirish

**Ma'lumotlar:**
- id, contract_id, invoice_number, materials[]
- total_amount, discount, fines[]
- issue_date, created_at

**Funksiyalar:**
- Faktura yaratish
- Materiallar ro'yxati
- Jarima (fine) qo'shish
- Chegirma (discount)
- PDF chop etish
- QR kod

---

### 4️⃣ Hisobotlar Moduli

**API Endpoints:**
- POST `/reports/general` - umumiy
- POST `/reports/materials-in-account` - hisobdagi
- POST `/reports/materials-with-customer` - mijozdagi
- POST `/reports/materials-in-warehouse` - ombordagi
- POST `/reports/cash-report` - kassa hisoboti
- POST `/reports/money-movements` - pul harakati
- POST `/reports/material-movements` - material harakati

**Funksiyalar:**
- Sana filtri (from_date, to_date)
- Filial filtri
- Excel/PDF export
- Grafiklar

---

### 5️⃣ Ombor Moduli

**API Endpoints:**
- POST `/warehouse/receive` - qabul qilish
- POST `/warehouse/dispatch` - chiqarish
- POST `/warehouse/transfer` - ko'chirish
- POST `/warehouse/stock` - qoldiq

**Funksiyalar:**
- Material qabul qilish
- Material chiqarish
- Filiallar o'rtasida ko'chirish
- Qoldiqni ko'rish
- Yuk beruvchilar bilan ishlash

---

### 6️⃣ Kassa Moduli

**API Endpoints:**
- POST `/cashbox/income` - kirim
- POST `/cashbox/expense` - chiqim
- POST `/cashbox/balance` - balans

**Ma'lumotlar:**
- id, type (income/expense), amount
- category_id, description, date
- payment_type (cash/card), branch_id

**Funksiyalar:**
- Pul kirim/chiqim
- Kategoriyalar (xarajat turlari)
- Balansni ko'rish
- Hisobot

---

### 7️⃣ Xodimlar Moduli

**API Endpoints:**
- POST `/users` - xodimlar ro'yxati
- POST `/roles` - rollar
- POST `/permissions` - ruxsatlar

**Ma'lumotlar:**
- id, full_name, username, role_id
- branch_id, permissions[], is_active

**Funksiyalar:**
- Xodimlar CRUD
- Rollar boshqaruvi
- Ruxsatlar (permissions)
- Filialga biriktirish

---

## 🔐 AUTENTIFIKATSIYA

### Login Jarayoni:
1. Login sahifa ochiladi
2. Username + Password kiritiladi
3. POST `/auth/login` - API ga yuboriladi
4. Response: `{success: true, token: "...", user_info: {...}}`
5. Token encrypted holda local storage ga saqlanadi
6. Keyingi requestlarda: `Authorization: Bearer {token}`
7. Token muddati tugasa, qayta login

### Token Management:
- **Service:** ITokenService
- **Methods:**
  - `GetTokenAsync()` - token olish
  - `SaveTokenAsync(token)` - saqlash
  - `ClearTokenAsync()` - o'chirish
- **Storage:** Encrypted local file

---

## 📊 MA'LUMOTLAR FORMATI

### Request Format (Example - Filial qidirish):
```json
POST /branches
Headers: {
  "Authorization": "Bearer eyJhbGc...",
  "Content-Type": "application/json"
}
Body: {
  "branch_name": "Toshkent"
}
```

### Response Format (Example - Filiallar ro'yxati):
```json
{
  "success": true,
  "resoult": [
    {
      "id": 1,
      "name": "Toshkent filiali",
      "description": "Asosiy filial",
      "type": "main",
      "responsible_worker": "Ahmad Ali",
      "created_at": "2024-01-15 10:30:00",
      "updated_at": "2024-01-20 14:00:00"
    }
  ]
}
```

### Pagination Format:
```json
{
  "success": true,
  "data": [...],
  "current_page": 1,
  "last_page": 5,
  "total": 47,
  "per_page": 10
}
```

### Error Format:
```json
{
  "success": false,
  "message": "Unauthorized",
  "errors": {
    "token": ["Token expired"]
  }
}
```

---

## 🔄 DATA FLOW

### Standard CRUD Flow:

```
┌──────────┐       ┌──────────────┐       ┌──────────┐       ┌────────┐
│  View    │──────▶│  ViewModel   │──────▶│ Service  │──────▶│  API   │
│ (XAML)   │◀──────│ (Commands)   │◀──────│ (HTTP)   │◀──────│(REST)  │
└──────────┘       └──────────────┘       └──────────┘       └────────┘
    ↑                      ↑                     ↑                ↑
    │                      │                     │                │
 Binding              ObservableCollection    JSON DTO        Database
```

1. **User** tugmani bosadi (View)
2. **Command** ishga tushadi (ViewModel)
3. **Service** method chaqiriladi
4. **HttpClient** API ga request yuboradi
5. **API** response qaytaradi (JSON)
6. **Service** JSON ni DTO ga parse qiladi
7. **ViewModel** ObservableCollection ni yangilaydi
8. **View** avtomatik update bo'ladi (Binding)

---

## 🎨 UI/UX PATTERN

### Page Struktura (Standart):

```xml
<Grid Background="...">
  <!-- Loading Overlay -->
  <Border Visibility="{Binding IsBusy}">
    <ProgressBar IsIndeterminate="True"/>
  </Border>

  <!-- Main Content -->
  <Grid>
    <RowDefinition Height="Auto"/>  <!-- Toolbar -->
    <RowDefinition Height="*"/>     <!-- Data Grid -->
  </Grid>

  <!-- Toolbar -->
  <Border Grid.Row="0">
    <Search Box + Action Buttons>
  </Border>

  <!-- Data Grid -->
  <Border Grid.Row="1">
    <DataGrid ItemsSource="{Binding Items}"/>
  </Border>

  <!-- Add/Edit Dialog -->
  <Border Visibility="{Binding IsDialogOpen}">
    <Form Fields + Save/Cancel Buttons>
  </Border>
</Grid>
```

---

## 📦 DEPENDENCY INJECTION

### App.xaml.cs da Registratsiya:

```csharp
// Singleton Services
services.AddSingleton<ITokenService, TokenService>();
services.AddSingleton<NotificationManager>();

// HTTP Clients with Factory
services.AddHttpClient<IAuthService, AuthService>();
services.AddHttpClient<IFilialService, FilialService>();
services.AddHttpClient<IMijozService, MijozService>();
// ... va hokazo

// ViewModels (Transient - har safar yangi instance)
services.AddTransient<FilialViewModel>();
services.AddTransient<MijozViewModel>();

// Views (Transient)
services.AddTransient<FilialPage>();
services.AddTransient<MijozPage>();
```

---

## 🚀 ASOSIY FUNKSIYALAR

1. ✅ **Admin autentifikatsiyasi** (login/logout)
2. ✅ **Filiallar boshqaruvi** (CRUD)
3. ✅ **Mijozlar boshqaruvi** (CRUD + qidiruv)
4. ✅ **Materiallar katalogi** (CRUD + kategoriyalar)
5. ✅ **Shartnoma tuzish** (materials + payment schedule)
6. ✅ **Faktura yaratish** (invoice + fines + discount)
7. ✅ **Kassa operatsiyalari** (income/expense tracking)
8. ✅ **Ombor boshqaruvi** (receive/dispatch/transfer)
9. ✅ **Hisobotlar** (date filter, export PDF/Excel)
10. ✅ **Xodimlar va ruxsatlar** (users + roles + permissions)

---

## 🔒 XAVFSIZLIK

- ✅ JWT token orqali autentifikatsiya
- ✅ Token encrypted saqlanadi
- ✅ Har bir API request da Authorization header
- ✅ Input validatsiya (frontend + backend)
- ✅ SQL injection protection (backend)
- ✅ Parollar hash qilingan (backend)
- ✅ Session timeout (token expiry)

---

## 📝 ESLATMALAR

- **Language:** Barcha UI o'zbek tilida
- **Admin only:** Public access yo'q
- **Offline mode:** Yo'q, internet kerak
- **Database:** Backend da (direct access yo'q)
- **Backup:** Backend javobgar
- **Multi-tenancy:** Har bir admin o'z filialini ko'radi (role-based)

---

**Versiya:** 3.0
**Sana:** 2025
**Status:** Development/Production Ready
