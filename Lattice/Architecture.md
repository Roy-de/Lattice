

```text
Lattice.slnx
│
├── Lattice              ← MAUI native application
├── Lattice.Shared       ← shared Blazor UI
├── Lattice.Web          ← ASP.NET Core web host
└── Lattice.Web.Client   ← Blazor WebAssembly client
```

This appears to be a template designed around **sharing the same Razor UI between MAUI Hybrid and web**.

That distinction is important for what you're building.

---

# 1. The four projects

Think of them like this:

```text
                         Lattice.Shared
                       Shared UI / Logic
                              │
                  ┌───────────┴───────────┐
                  │                       │
                  ▼                       ▼
             Lattice                  Lattice.Web
          MAUI application           Web application
                  │                       │
                  ▼                       ▼
           BlazorWebView          ASP.NET Core
                  │                       │
                  ▼                       ▼
             Native app             Web browser
```

And `Lattice.Web.Client` participates in the web side.

So you aren't looking at "just a MAUI project."

You're looking at a solution designed to support:

**one shared Blazor UI → multiple application hosts.**

---

# 2. `Lattice/`

This is your actual **.NET MAUI application**.

The important files are:

```text
Lattice/
│
├── App.xaml
├── App.xaml.cs
├── MainPage.xaml
├── MainPage.xaml.cs
├── MauiProgram.cs
│
├── Components/
│   └── _Imports.razor
│
├── Platforms/
├── Resources/
├── Services/
└── wwwroot/
```

This is the native application.

When you run:

```text
Lattice → Windows
```

you get a Windows application.

When you run:

```text
Lattice → Android
```

you get an Android application.

etc.

---

# 3. `Lattice/MainPage.xaml`

This is particularly important.

This is the **native MAUI page that hosts your Blazor UI**.

Conceptually:

```text
Lattice
   │
   └── MainPage.xaml
          │
          └── BlazorWebView
                  │
                  ▼
            Lattice.Shared
```

So don't think of `MainPage.xaml` as where you'll build your application screens.

Instead:

> `MainPage.xaml` is the doorway from MAUI into Blazor.

---

# 4. `Lattice.Shared/`

This is probably the most important project for your idea.

```text
Lattice.Shared/
│
├── Routes.razor
├── _Imports.razor
│
├── Layout/
│   ├── MainLayout.razor
│   ├── MainLayout.razor.css
│   ├── NavMenu.razor
│   └── NavMenu.razor.css
│
├── Pages/
│   ├── Home.razor
│   ├── Counter.razor
│   ├── Weather.razor
│   └── NotFound.razor
│
├── Services/
│   └── IFormFactor.cs
│
└── wwwroot/
    ├── app.css
    ├── favicon.png
    └── lib/
```

This is your **shared UI layer**.

For example:

```text
Pages/Home.razor
```

can be used by both:

```text
MAUI
 +
Web
```

That's the point of the project.

---

# 5. `Lattice.Web/`

This is the **ASP.NET Core web host**.

```text
Lattice.Web/
│
├── Program.cs
├── appsettings.json
│
├── Components/
│   ├── App.razor
│   └── Pages/
│       └── Error.razor
│
└── Services/
    └── FormFactor.cs
```

This is responsible for hosting the web version.

Think:

```text
Browser
   ↓
Lattice.Web
   ↓
Lattice.Shared
```

Whereas the MAUI version is:

```text
Native application
   ↓
Lattice
   ↓
BlazorWebView
   ↓
Lattice.Shared
```

Same UI.

Different host.

---

# 6. `Lattice.Web.Client/`

This is the browser-side Blazor project.

```text
Lattice.Web.Client/
│
├── Program.cs
├── _Imports.razor
│
├── Layout/
│   ├── ReconnectModal.razor
│   ├── ReconnectModal.razor.css
│   └── ReconnectModal.razor.js
│
├── Services/
│   └── FormFactor.cs
│
└── wwwroot/
```

This exists because the web application is using the newer Blazor architecture where the client-side part is separated from the ASP.NET Core host.

You don't necessarily need to modify this immediately.

---

# 7. The really important relationship

Your solution currently looks approximately like:

```text
                    Lattice.Shared
                    ──────────────
                    Razor UI
                    Components
                    Layouts
                    Pages
                    Shared services
                         ▲
                         │
              ┌──────────┴──────────┐
              │                     │
              │                     │
        Lattice MAUI           Lattice.Web
        ───────────           ──────────
        Native host           Web host
              │                     │
              ▼                     ▼
        BlazorWebView          ASP.NET Core
              │                     │
              ▼                     ▼
          Desktop/             Browser
          Mobile
```

This is a **very useful architecture** for your project.

---

# 8. Why `Lattice.Shared` matters for your idea

Suppose you create:

```text
Lattice.Shared/
    Components/
        Dynamic/
            DynamicRenderer.razor
            DynamicText.razor
            DynamicButton.razor
            DynamicCard.razor
            DynamicChart.razor
```

Then your renderer can be used by:

```text
                 DynamicRenderer
                       │
          ┌────────────┴────────────┐
          │                         │
          ▼                         ▼
      MAUI app                  Web app
```

You don't have to implement the UI engine twice.

That's one of the strongest aspects of this architecture.

---

# 9. Notice something else in your tree

Your MAUI project has:

```text
Lattice/wwwroot/
    app.css
    index.html
```

while the shared project has:

```text
Lattice.Shared/wwwroot/
    app.css
    favicon.png
    lib/
```

This is because **static web assets can come from referenced projects**.

The build system combines/serves those assets appropriately for the host.

So don't immediately assume:

> "There are two `wwwroot` folders, therefore there are two completely separate web applications."

They have different responsibilities.

---

# 10. `Platforms/`

This part is purely MAUI/native.

```text
Lattice/Platforms/

├── Android/
│   ├── AndroidManifest.xml
│   ├── MainActivity.cs
│   └── MainApplication.cs
│
├── iOS/
│   ├── AppDelegate.cs
│   ├── Info.plist
│   └── Program.cs
│
├── MacCatalyst/
│
└── Windows/
    ├── app.manifest
    ├── App.xaml
    ├── App.xaml.cs
    └── Package.appxmanifest
```

You can largely ignore this while learning Blazor.

You only go here when you need platform-specific behavior.

For example:

```text
Camera
Bluetooth
Push notifications
Windows APIs
Android permissions
iOS capabilities
```

---

# 11. `Services/`

You currently have:

```text
Lattice/Services/FormFactor.cs
Lattice.Shared/Services/IFormFactor.cs
Lattice.Web/Services/FormFactor.cs
Lattice.Web.Client/Services/FormFactor.cs
```

This is actually a good example of the architecture.

The shared project defines the abstraction:

```text
IFormFactor
```

while the different hosts provide their own implementation.

Conceptually:

```text
                  IFormFactor
                     │
          ┌──────────┼───────────┐
          │          │           │
          ▼          ▼           ▼
        MAUI        Web       Web Client
      FormFactor  FormFactor   FormFactor
```

This is dependency inversion applied to the host-specific functionality.

---

# 12. What you can ignore for now

Most of this:

```text
bin/
obj/
.idea/
```

is build/IDE generated material.

For understanding the architecture, mentally remove:

```text
bin/
obj/
.idea/
```

Your actual project becomes:

```text
Lattice.slnx
│
├── Lattice
│   ├── MauiProgram.cs
│   ├── App.xaml
│   ├── MainPage.xaml
│   ├── Platforms/
│   ├── Resources/
│   ├── Services/
│   └── wwwroot/
│
├── Lattice.Shared
│   ├── Pages/
│   ├── Layout/
│   ├── Services/
│   ├── Routes.razor
│   └── wwwroot/
│
├── Lattice.Web
│   ├── Program.cs
│   ├── Components/
│   └── Services/
│
└── Lattice.Web.Client
    ├── Program.cs
    ├── Layout/
    ├── Services/
    └── wwwroot/
```

That's the structure you should learn.

---

## The key idea

Your solution isn't simply:

```text
MAUI
 └── Blazor
```

It's closer to:

```text
                  SHARED UI
                Lattice.Shared
                      │
              ┌───────┴────────┐
              │                │
           MAUI HOST         WEB HOST
              │                │
         Lattice          Lattice.Web
              │                │
       BlazorWebView      Web infrastructure
              │                │
              └───────┬────────┘
                      │
                Same Razor UI
```
