# Самостійна робота №3

**Тема:** Аналіз інкапсуляції в open-source проєктах. Практики валідації полів.


## Звіт з аналізу інкапсуляції в Open-Source проєкті

### 1. Обраний проєкт

- **Назва:** Entity Framework Core (EF Core)
- **Посилання на GitHub:** [https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)


### 2. Аналіз інкапсуляції

#### Клас 1: `DbContext`
- **Посилання на файл:** [DbContext.cs](https://github.com/dotnet/efcore/blob/main/src/EFCore/DbContext.cs)
- **Опис класу:** Центральний клас у EF Core, який відповідає за взаємодію з базою даних, управління сесіями та відстеження змін об'єктів.
- **Поля:** Використовуються приватні поля з підкресленням для зберігання внутрішніх сервісів, наприклад:
  ```csharp
  private DbContextOptions _options;
  private IServiceProvider _serviceProvider;
  private bool _disposed;