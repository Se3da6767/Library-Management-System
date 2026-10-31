# LibrarySystem — مشروع ASP.NET Core MVC

هذا المشروع مبني بنفس نمط مشروع الشرح، ويضم:

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core + SQL Server
- CRUD كامل لـ Books / Categories / Publishers / Members / Borrowings
- علاقات One-to-Many متعددة
- DataAnnotations وModelState custom validation
- SelectList وInclude وNavigation Properties
- Search + Sorting مع الحفاظ على البحث أثناء الترتيب
- رفع وتعديل وحذف صور أغلفة الكتب باستخدام IFormFile + GUID
- No_Image.png افتراضية
- DisplayNameFor / DisplayFor
- Partial View للدعم الفني
- Bootstrap + Bootstrap Icons (من CDN — يحتاج اتصال بالإنترنت لعرض التصميم والأيقونات)
- ViewBag في Home وCategory Details
- Seed Data عبر HasData

## قبل التشغيل

1. افتح `LibrarySystem.sln` في Visual Studio 2026.
2. تأكد أن `.NET 9 SDK` مثبت.
3. راجع اسم السيرفر في `appsettings.json` (تم نقل الـ Connection String من `AppDbContext` إلى هنا):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-JJ1G2HH\\SQLEXPRESS;Database=LibrarySystem;Trusted_Connection=True;TrustServerCertificate=True"
}
```

4. المشروع يحتوي بالفعل على Migrations حقيقية داخل مجلد `Migrations`، لذلك لا تنشئ `InitialCreate` جديدة. من Package Manager Console نفّذ فقط:

```powershell
Update-Database
```

إذا كنت قد أنشأت قاعدة البيانات من نسخة أقدم من المشروع قبل تصحيح الـMigrations، ولم تكن لديك بيانات مهمة تريد الاحتفاظ بها، أعد إنشاء قاعدة البيانات حتى تطابق الـMigration history الحالية:

```powershell
Drop-Database
Update-Database
```

5. شغّل المشروع بـ `Ctrl + F5`.

## ترتيب الاختبار المقترح

1. Categories
2. Publishers
3. Books + image upload
4. Members
5. Borrowings
6. Search + Sorting داخل Books
7. Details / Edit / Delete

## ملاحظة

ملف `No_Image.png` موجود داخل:

`wwwroot/images/No_Image.png`

## Migration History

المشروع يحتوي على تسلسل Migrations واضح يمثل تطور المشروع:

1. `InitialCreate` — إنشاء الجداول والعلاقات الأساسية.
2. `AddBookStockQuantity` — إضافة `StockQuantity` إلى `Books`.
3. `AddMembershipType` — إضافة `MembershipType` إلى `Members`.
4. `AddBorrowingLateFee` — إضافة `LateFee` إلى `Borrowings`.


## التحسينات في النسخة الحالية

**إصلاحات:**
- صفحة Home كانت ناقصة (الضغط على Home كان يعطي خطأ) → أصبحت Dashboard فيها إحصائيات وآخر الاستعارات، وهي الصفحة الافتراضية الآن.
- حذف عضو له سجل استعارات مُرجعة كان يسبب خطأ من SQL Server (بسبب `DeleteBehavior.NoAction`) → أصبح الحذف ممنوعاً برسالة واضحة، مع اقتراح تعطيل العضو بدلاً من حذفه.
- ملف الأيقونات المحلي كان يعرض رموز Emoji بدلاً من Bootstrap Icons → تم استبداله بالمكتبة الحقيقية.
- `StockQuantity` كان موجوداً في الـ Model وقاعدة البيانات لكن غير موجود في الفورم → أُضيف للإضافة والتعديل والتفاصيل.

**تحسينات في الكود:**
- `AppDbContext` يُسجَّل عبر Dependency Injection (`AddDbContext`) بدلاً من `new()` في كل Controller، والـ Connection String في `appsettings.json`.
- التحقق من صورة الغلاف: الامتدادات المسموحة (jpg/jpeg/png/webp) وحد أقصى 2MB، ومسار الصورة القديمة يُقرأ من قاعدة البيانات وليس من الحقل المخفي في الفورم.
- Client-side validation باستخدام jQuery Validation Unobtrusive.
- رسائل نجاح بعد الإضافة والتعديل والحذف (TempData).

**ميزات صغيرة في الاستعارات:**
- زر "Mark as returned" بضغطة واحدة: يسجّل تاريخ الإرجاع، يحسب غرامة التأخير تلقائياً (5 لكل يوم)، ويعيد الكتاب متاحاً.
- فلترة الاستعارات: All / On loan / Overdue / Returned، مع شارة Overdue بعدد أيام التأخير.

**الواجهة:**
- تصميم موحد بقائمة جانبية، عناوين صفحات، جداول وبطاقات وشارات حالة، وفورمات بعمودين، وصفحات حذف بتحذير واضح.
- الترتيب في جدول الكتب بالضغط على عنوان العمود مع إظهار الاتجاه الحالي.
- Partial Views للحقول المشتركة بين Create و Edit (`_BookFields`, `_MemberFields`, ...).

> لا توجد تغييرات على الـ Models، لذلك **لا تحتاج Migration جديدة**.
