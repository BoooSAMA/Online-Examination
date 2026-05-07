[English](README.md)

# Online Examination（在线考试系统）

基于 **ASP.NET Core Blazor Interactive Server** 的在线考试 Web 应用。使用 C#、Entity Framework Core 和 SQL Server 构建。

---

## 技术栈

| 层级 | 技术 |
|------|------|
| 框架 | ASP.NET Core 8.0 |
| 前端 | Blazor Interactive Server（服务端渲染 + SignalR） |
| 语言 | C# 12、HTML/Razor、CSS |
| 数据库 | SQL Server |
| ORM | Entity Framework Core 8.0 |
| 身份认证 | ASP.NET Core Identity（基于角色：管理员 / 学生） |
| 图表 | Chart.js（CDN） |
| 邮件 | Gmail SMTP |

### NuGet 包

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.22
- `Microsoft.AspNetCore.Identity.UI` 8.0.22
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.22
- `Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter` 8.0.22
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` 8.0.22

---

## 功能特性

### 管理员端
- **仪表盘** — 数据分析图表（Chart.js）
- **创建考试** — 设置标题、描述、限时（1–180 分钟）、访问码、学段、科目
- **管理题目** — 单选题（A–D 四个选项）、正确答案、可选图片上传、可选阅读材料
- **管理学生** — 查看、编辑、删除学生账号
- **全局历史** — 查看所有学生在所有考试中的答题记录
- **自动评分** — 提交后即时计算得分

### 学生端
- **加入考试** — 通过唯一的 8 位访问码参加考试
- **参加考试** — 计时答题界面，支持逐题浏览或全页展示
- **考试历史** — 查看历史成绩
- **模拟测试** — 按学段分类的模拟练习（PSLE / N 水准 / O 水准 / Poly / JC）
- **自动生成数学题** — 4 个难度级别（简单 → 专家）的程序化数学题生成

### 通用
- **基于角色的导航** — 管理员和学生显示不同的菜单
- **注册与登录**（含邮箱验证）
- **忘记 / 重置密码**（通过 Gmail SMTP）

---

## 系统架构

```
┌──────────────────────────────────────────────┐
│             Blazor Components（UI 层）        │
│   页面、布局、导航菜单、Razor 组件             │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│          Controllers（REST API 层）           │
│       POST /api/login、/api/auth/*            │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│            Services（业务逻辑层）              │
│  ExamService、StudentService、UserSession、   │
│  GmailEmailSender、LocalMathGenerator         │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│        Data / Entity Framework Core（数据层）  │
│      Online_ExaminationContext、Migrations    │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│            SQL Server 数据库                  │
└──────────────────────────────────────────────┘
```

---

## 数据模型

```
Online_ExaminationUser（继承 IdentityUser）
    │
    ├── CreatedExams（1:N）──→ Exam（考试）
    │       │
    │       ├── Questions（1:N）──→ Question（题目）
    │       │     • 题目文本、选项 A–D、正确答案
    │       │     • 可选：图片链接、阅读材料
    │       │
    │       └── Attempts（1:N）──→ Attempt（答题记录）
    │             • 用户外键、考试外键、得分
    │
    └── Attempts（1:N）
```

**BaseDomainModel** — 抽象基类，包含 `Id`、`DateCreated`、`DateUpdated`、`CreatedBy`、`UpdatedBy`。

---

## 环境要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)（LocalDB、Express 或 Developer 版）
- [Visual Studio 2022](https://visualstudio.microsoft.com/)（推荐）或其他代码编辑器

---

## 快速开始

### 1. 克隆仓库

```bash
git clone https://github.com/BoooSAMA/Online-Examination.git
cd Online-Examination
```

### 2. 配置数据库连接字符串

编辑 `Online Examination/appsettings.json`，指向你的 SQL Server 实例：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=你的服务器;Database=OnlineExaminationDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. 配置 Gmail SMTP（用于密码重置）

编辑 `Online Examination/Services/GmailEmailSender.cs`，替换凭据：

```csharp
// 替换为你自己的 Gmail 和应用专用密码
private const string Email = "你的邮箱@gmail.com";
private const string AppPassword = "你的应用专用密码";
```

> **安全提示**：生产环境中，请将凭据存储在环境变量或 Azure Key Vault 中——切勿硬编码在源码里。

### 4. 运行数据库迁移

```bash
cd "Online Examination"
dotnet ef database update
```

或在 Visual Studio 的包管理器控制台中执行：

```
Update-Database
```

### 5. 运行应用

```bash
dotnet run
```

或在 Visual Studio 中按 `F5`。

应用将在 `https://localhost:5001`（或 `launchSettings.json` 中指定的端口）启动。

---

## 默认账号

首次运行时，数据库种子数据会自动创建以下账号：

| 角色 | 邮箱 | 密码 |
|------|------|------|
| 管理员 | `admin@test.com` | `Admin123` |
| 学生 | `student@test.com` | `Student123` |

---

## 项目结构

```
Online-Examination/
├── Online Examination.slnx                    # 解决方案文件
├── README.md
│
└── Online Examination/                        # 主项目
    ├── Program.cs                             # 入口、依赖注入、中间件
    ├── appsettings.json                       # 配置文件
    │
    ├── Domain/                                # 实体模型
    │   ├── BaseDomainModel.cs                 # 抽象基类
    │   ├── Exam.cs                            # 考试实体
    │   ├── Question.cs                        # 题目实体
    │   ├── Attempt.cs                         # 答题记录实体
    │   └── Online_ExaminationUser.cs           # 用户实体
    │
    ├── Data/                                  # EF Core 数据库上下文 + 种子数据
    │   ├── Online_ExaminationContext.cs
    │   └── DatabaseSeeder.cs
    │
    ├── Migrations/                            # EF Core 迁移文件
    │
    ├── Controllers/                           # REST API 控制器
    │   ├── LoginController.cs                 # 登录接口
    │   └── AuthController.cs                  # 认证接口（忘记/重置密码）
    │
    ├── Services/                              # 业务逻辑层
    │   ├── ExamService.cs                     # 考试 CRUD、评分
    │   ├── StudentService.cs                  # 学生注册、登录、答题、历史
    │   ├── UserSession.cs                     # 用户会话状态
    │   ├── GmailEmailSender.cs                # 邮件发送
    │   └── QuestionGenerators/
    │       └── LocalMathGenerator.cs          # 数学题自动生成器
    │
    ├── Components/                            # Blazor UI
    │   ├── Layout/
    │   │   ├── MainLayout.razor               # 主布局
    │   │   └── NavMenu.razor                  # 导航菜单（基于角色）
    │   ├── Pages/
    │   │   ├── Home.razor                     # 首页
    │   │   ├── Login.razor                    # 登录页
    │   │   ├── AdminDashboard.razor           # 管理员仪表盘
    │   │   ├── UserDashboard.razor            # 学生仪表盘
    │   │   ├── ExamCreate.razor               # 创建考试
    │   │   ├── ExamPage.razor                 # 考试答题页
    │   │   ├── ExamResult.razor               # 考试结果
    │   │   ├── JoinExam.razor                 # 加入考试
    │   │   ├── ExamHistory.razor              # 历史记录
    │   │   ├── Payment.razor                  # 支付页
    │   │   └── ...
    │   └── Account/                           # Identity 脚手架页面
    │
    └── wwwroot/                               # 静态资源
        ├── app.css
        ├── css/
        │   ├── exams-page.css
        │   ├── indexstyle.css
        │   └── site.css
        └── pics/
```

---

## API 接口

| 方法 | 接口 | 说明 |
|------|------|------|
| POST | `/api/login` | 用户登录 |
| POST | `/api/auth/forgot-password` | 发送密码重置邮件 |
| POST | `/api/auth/reset-password` | 使用令牌重置密码 |

---

## 页面路由

| 路由 | 页面 | 访问权限 |
|------|------|----------|
| `/` | 首页 | 公开 |
| `/about` | 关于我们 | 公开 |
| `/Account/Login` | 登录 | 公开 |
| `/Account/Register` | 注册 | 公开 |
| `/forgot-password` | 忘记密码 | 公开 |
| `/admin-dashboard` | 管理员仪表盘 | 管理员 |
| `/admin/exam-create` | 创建考试 | 管理员 |
| `/exam-index` | 管理考试 | 管理员 |
| `/modify-students` | 管理学生 | 管理员 |
| `/modify-exam/{id}` | 编辑考试 | 管理员 |
| `/admin/global-history` | 全局历史 | 管理员 |
| `/user-dashboard` | 学生仪表盘 | 学生 |
| `/student/join-exam` | 加入考试 | 学生 |
| `/exams` | 可用考试列表 | 学生 |
| `/take-exam/{id}` | 参加考试 | 学生 |
| `/exam-history` | 我的历史 | 学生 |
| `/exam-result/{id}` | 查看结果 | 两者 |
| `/payment` | 支付 | 学生 |
| `/mock-test/{level}` | 模拟测试 | 学生 |

---

## 数据库迁移历史

| 迁移 | 日期 | 变更内容 |
|------|------|----------|
| `InitialCreate` | 2026-01-16 | 创建基础表结构 |
| `AddAccessCodeToExam` | 2026-01-18 | 为考试添加唯一访问码 |
| `AddEducationLevelToExam` | 2026-01-18 | 添加学段字段 |
| `AddExamSubject` | 2026-01-21 | 添加科目分类 |
| `AddJCLevel` | 2026-01-21 | 添加初级学院（JC）学段 |

---

## 安全注意事项

- ⚠️ **邮件凭据硬编码在源码中**（`GmailEmailSender.cs`）。部署到生产环境前，务必将凭据移至环境变量或密钥管理器。
- 密码策略在开发阶段有意放宽。生产环境中请在 `Program.cs` 中加强 `PasswordOptions` 配置。
- 登录接口使用了 `[IgnoreAntiforgeryToken]`——请评估你的部署是否需要此标记。

---

## 许可证

本项目为开源项目。如需分发，请添加许可证文件。
