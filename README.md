# Contacts

Простое WPF-приложение на C# по паттерну MVVM для ввода, валидации, сохранения и загрузки контактных данных.

## Возможности

- ввод имени, телефона и email
- валидация полей в реальном времени
- сохранение контакта в JSON
- загрузка контакта из JSON
- привязка данных через MVVM

## Стек

- C#
- WPF
- .NET
- Newtonsoft.Json

## Структура проекта

- `Models/` — модель контакта
- `Models/Services/` — сериализация и работа с JSON
- `ViewModels/` — основная логика и команды
- `Commands/` — вспомогательные команды
- `Converters/` — конвертеры для UI
- `Assets/` — изображения и иконки
- `MainWindow.xaml` — интерфейс приложения

## Запуск

### Visual Studio

Откройте `Contacts.sln` и запустите проект.

### CLI

```bash
dotnet restore
dotnet build
dotnet run --project Contacts.csproj
```

### Требования

- Windows
- .NET SDK с поддержкой net10.0-windows
- Visual Studio с workload .NET desktop development

### Где хранятся данные

После сохранения создаётся файл:

Documents/Contacts/contacts.json
