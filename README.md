# 🌳 Tree API

**Tree API** — это ASP.NET Core приложение, предоставляющее REST-интерфейс для работы с деревьями, узлами, журналами и авторизацией пользователей по коду.

## 📦 Возможности API

---

### 🧾 user.journal — журнал действий

- `POST /api.user.journal.getRange`  
  Получение событий с пагинацией. Параметры: `skip`, `take`, фильтр в теле запроса.

- `POST /api.user.journal.getSingle`  
  Получение одного события по ID (`id` в query).

---

### 🌲 user.tree — дерево пользователя

- `POST /api.user.tree.get`  
  Возвращает всё дерево пользователя по имени. Создаёт его при отсутствии. Параметр: `treeName`.

---

### 🌿 user.tree.node — работа с узлами

- `POST /api.user.tree.node.create`  
  Создание узла в дереве. Параметры: `treeName`, `parentNodeId`, `nodeName`. Имя должно быть уникальным среди "соседей".

- `POST /api.user.tree.node.delete`  
  Удаление узла. Параметры: `treeName`, `nodeId`.

- `POST /api.user.tree.node.rename`  
  Переименование узла. Параметры: `treeName`, `nodeId`, `newNodeName`.


## ✅ Тесты

В проекте реализованы модульные и интеграционные тесты с использованием:
- `xUnit`
- `Microsoft.AspNetCore.Mvc.Testing`
- `CustomWebApplicationFactory` для запуска тестового сервера.
- Используется фикстура `IClassFixture`, переиспользуемая между тестами.
- Генерация клиента API с помощью `NSwag`.

Запуск тестов: ( предварительно подняв сервис postgres в докере)
```bash
docker compose up postgres

```bash
dotnet test
