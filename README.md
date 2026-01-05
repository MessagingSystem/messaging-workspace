# messaging-workspace

Umbrella-репозиторий для разработки набора библиотек “Messaging*” и связанных пакетов.

Содержит:
- git submodules на остальные репозитории
- общую Solution `Messaging.Workspace.sln`
- `samples/` (демо сервисы)
- `tests/` (интеграционные тесты)

Рабочие проекты/продукты обычно потребляют пакеты через NuGet/npm, а не исходники.
Workspace нужен для удобной совместной разработки и сквозных тестов.

## Структура

Ожидаемая структура:
- `src/abstractions` → submodule `messaging-abstractions`
- `src/hosting-aspnetcore` → submodule `messaging-hosting-aspnetcore`
- `src/providers/telegram` → submodule `messaging-telegram`
- `src/providers/max` → submodule `messaging-max`
- `packages/telegram-miniapp-sdk` → submodule `telegram-miniapp-sdk`
- `packages/telegram-miniapp-auth` → submodule `telegram-miniapp-auth`

## Клонирование

Если клоните workspace впервые:

```bash
git clone --recurse-submodules <REPO_URL>
```

Если уже клонили без submodules:

```bash
git submodule update --init --recursive
```

## Обновление submodules

Обновить все submodules до последних коммитов их текущих веток:

```bash
git submodule foreach 'git checkout main && git pull'
git add .
git commit -m "chore: bump submodules"
```

Важно: workspace хранит “пин” на конкретные коммиты submodule’ов. Это ожидаемое поведение.

## Сборка и тесты

```bash
dotnet build Messaging.Workspace.sln
dotnet test Messaging.Workspace.sln
```

Если есть фронтовой пакет:

```bash
cd packages/telegram-miniapp-sdk
npm ci
npm run build
```

## Как вносить изменения

Правильный поток:
- Изменения делаются в репозитории сабмодуля (например `messaging-telegram`)
- PR/merge в репозитории сабмодуля
- В workspace обновляете указатель submodule на новый коммит и коммитите это в workspace

## Добавление нового провайдера

Рекомендуемый путь:
- Создать новый репозиторий `messaging-<provider>`
- Реализовать `IChatClient` из `Messaging.Abstractions`
- Подключить репо как submodule в workspace в `src/providers/<provider>`
- Добавить проекты в `Messaging.Workspace.sln`
- Добавить хотя бы один интеграционный тест
