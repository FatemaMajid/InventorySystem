const ar = {
  common: {
    systemName: 'نظام إدارة المخزون',
    user: 'المستخدم',
    inventoryUser: 'مستخدم الجرد',
    logout: 'تسجيل الخروج',
    notifications: 'الإشعارات',
    lightMode: 'الوضع الفاتح',
    darkMode: 'الوضع الداكن',
  },

  navigation: {
    dashboard: 'لوحة التحكم',
    inventorySessions: 'جلسات الجرد',
    newInventorySession: 'جلسة جرد جديدة',
    comparisonResults: 'نتائج المقارنة',
    attentionItems: 'العناصر التي تحتاج انتباه',
    reports: 'التقارير',
    branches: 'الفروع',
    stores: 'المخازن',
    users: 'المستخدمون',
    rolesPermissions: 'الأدوار والصلاحيات',
    auditLogs: 'سجل العمليات',
    settings: 'الإعدادات',
  },

  navigationGroups: {
    main: 'الرئيسية',
    inventory: 'الجرد',
    results: 'النتائج',
    masterData: 'البيانات الأساسية',
    administration: 'الإدارة',
    system: 'النظام',
  },

  inventory: {

    inventoryType: 'نوع الجرد',

selectInventoryType:
  'اختر نوع الجرد',

inventoryTypes: {
  semiAnnual: 'الجرد نصف السنوي',
  annual: 'الجرد السنوي',
},

beforeInventory: 'قبل الجرد',

beforeInventoryDescription:
  'ارفع ملف Excel قبل بدء عملية الجرد.',

afterInventory: 'بعد الجرد',

afterInventoryDescription:
  'ارفع ملف Excel بعد إكمال عملية الجرد.',

invalidExcelFile:
  'يسمح فقط بملفات Excel بصيغة .xlsx أو .xls.',

removeFile: 'إزالة الملف',

checkingFile:
  'جاري التحقق من الملف...',

fileValidated:
  'تم التحقق من الملف بنجاح',

totalRows: 'إجمالي الصفوف',

validRows: 'الصفوف الصحيحة',

errorRows: 'الصفوف التي تحتوي أخطاء',

previewError:
  'فشل التحقق من ملف Excel.',

createSessionError:
  'فشل إنشاء جلسة الجرد.',

creatingSession:
  'جاري إنشاء الجلسة...',
  
    sessionInformation: 'معلومات جلسة الجرد',
    sessionInformationDescription:
      'أدخل المعلومات الأساسية لجلسة الجرد الجديدة.',

    inventoryType: 'نوع الجرد',
    selectInventoryType: 'اختر نوع الجرد',

    inventoryTypes: {
      semiAnnual: 'الجرد نصف السنوي',
      annual: 'الجرد السنوي',
    },


    requiredColumns: 'الأعمدة المطلوبة',
    requiredColumnsDescription:
      'يجب أن يحتوي كلا ملفي Excel على الأعمدة التالية.',

    requiredColumnNames: {
      itemCode: 'رقم الصنف',
      itemName: 'اسم الصنف',
      category: 'التصنيف',
      quantity: 'الكمية',
      price: 'السعر',
    },

    selectBranch: 'اختر الفرع',
    selectStore: 'اختر المخزن',

    inventoryDate: 'تاريخ الجرد',

    notes: 'ملاحظات',
    notesPlaceholder:
      'أضف أي ملاحظات خاصة بجلسة الجرد.',

    uploadFile: 'رفع ملف الجرد',
    uploadFileDescription:
      'ارفع ملف الجرد لبدء عملية التحقق.',

    beforeInventory: 'قبل الجرد',
    beforeInventoryDescription:
      'ارفع ملف الإكسل قبل بدء الجرد.',

    afterInventory: 'بعد الجرد',
    afterInventoryDescription:
      'ارفع ملف الإكسل بعد إتمام الجرد.',

    chooseFile: 'اختيار ملف',
    dropFile: 'اسحب الملف هنا',
    or: 'أو',

    requiredColumns: 'الأعمدة المطلوبة',
    requiredColumnsDescription:
      'يجب أن يحتوي الملف المرفوع على الأعمدة التالية.',

    createSession: 'إنشاء الجلسة',
    cancel: 'إلغاء',
    invalidExcelFile: 'يسمح فقط بملفات Excel بصيغة .xlsx أو .xls.',
  },

  home: {
    welcomeLabel: 'مرحباً بعودتك',
    title: 'نظام إدارة المخزون',
    subtitle:
      'إدارة جلسات الجرد، مراجعة النتائج، ومتابعة عمليات المخزون.',

    quickActions: 'إجراءات سريعة',
    quickActionsSubtitle:
      'الوصول إلى عمليات المخزون الأكثر استخداماً.',

    newInventory: 'جلسة جرد جديدة',
    newInventoryDescription:
      'بدء جلسة جرد جديدة.',

    inventorySessions: 'جلسات الجرد',
    inventorySessionsDescription:
      'عرض وإدارة جلسات الجرد.',

    reports: 'التقارير',
    reportsDescription:
      'مراجعة وتصدير تقارير الجرد.',

    overview: 'نظرة عامة',
    overviewSubtitle:
      'نظرة سريعة على نظام إدارة المخزون.',

    activeSessions: 'الجلسات النشطة',
    branches: 'الفروع',
    attentionItems: 'العناصر التي تحتاج انتباه',
  },
};

export default ar;