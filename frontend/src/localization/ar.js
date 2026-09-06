const ar = {
  common: {
    systemName: 'نظام إدارة المخزون',
    user: 'المستخدم',
    inventoryUser: 'مستخدم الجرد',
    logout: 'تسجيل الخروج',
    notifications: 'الإشعارات',
    lightMode: 'الوضع الفاتح',
    darkMode: 'الوضع الداكن',
    loading: "جارٍ التحميل...",
    noResults: "لا توجد نتائج.",
    error: "حدث خطأ ما.",
    retry: "إعادة المحاولة",
  },

  navigation: {
    home: 'الصفحة الرئيسية',
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
    activeSessionsDescription: 'قيد التنفيذ حالياً',

    totalSessionsDescription: 'جميع جلسات الجرد',

    attentionItemsDescription: 'تتطلب المراجعة',
    totalSessions: 'إجمالي الجلسات',

    recentSessions: {
      title: "جلسات الجرد الأخيرة",
      viewAll: "عرض جميع الجلسات",

      sessionId: "رقم الجلسة",
      date: "التاريخ",
      branch: "الفرع",
      store: "المستودع",
      status: "الحالة",
      items: "العناصر",
      actions: "الإجراءات",
      view: "عرض",
      noSessions: "لا توجد جلسات جرد",

      statuses: {
        completed: "مكتملة",
        inProgress: "قيد التنفيذ",
        cancelled: "ملغاة",
      },
    },
    itemCategories: "تصنيفات الأصناف",
    stores: "المستودعات",
    totalBranches: "إجمالي الفروع",
    totalStores: "إجمالي المخازن",
    totalCategories: "إجمالي التصنيفات",
    totalItems: "إجمالي الأصناف",

    systemStatus: 'حالة النظام',
    apiStatus: 'حالة واجهة API',
    database: 'قاعدة البيانات',
    lastBackup: 'آخر نسخة احتياطية',
    systemHealth: 'سلامة النظام',
    connected: 'متصل',
    healthy: 'سليم',
    lastBackupValue: 'اليوم، 03:00 م',
  },

  dashboard: {
    header: {
      title: 'لوحة تحكم الجرد',
      session: 'الجلسة',
      statuses: {
        completed: 'مكتملة',
        inProgress: 'قيد التنفيذ',
      },
      type: 'النوع',
      inventoryTypes: {
        semiAnnual: 'جرد نصف سنوي',
        annual: 'جرد سنوي',
      },
      date: 'التاريخ',
      branch: 'الفرع',
      store: 'المخزن',
      exportExcel: 'تصدير Excel',
      exportPdf: 'تصدير PDF',
    },

    kpiCards: {
      totalItems: 'إجمالي العناصر',
      increase: 'زيادة',
      decrease: 'نقص',
      match: 'مطابقة',
      newlyCounted: 'ظهر بعد الجرد',
      fullyDepleted: 'غير موجود بعد الجرد ',
      priceChanged: 'تغير في السعر',
      unitNotDefined: 'الوحدة غير معرفة',
      title: 'حالة العناصر',
      total: 'المجموع',

      topDifferences: {
        title: 'أعلى 10 مراكز حسب فرق القيمة (د.ع)'
      }
    },

    financialSummary: {
      title: "ملخص مالي",
      totalValueBefore: "إجمالي القيمة قبل الجرد",
      totalValueAfter: "إجمالي القيمة بعد الجرد",
      totalDifference: "إجمالي الفرق",
      differencePercentage: "نسبة الفرق",
      currency: 'د.ع',
    },

    valueChart: {
      title: "قيمة المخزون",
      subtitle: "مقارنة قيمة المخزون",
      before: "قبل الجرد",
      after: "بعد الجرد",
    },

    attention: {
      title: 'تحتاج إلى انتباه',
      description: 'عناصر تحتاج إلى انتباهك',
      viewAll: 'عرض الكل',
      view: 'عرض',
      newlyCounted: 'ظهر بعد الجرد',
      fullyDepleted: 'غير موجود بعد الجرد',
      unitNotDefined: 'الوحدة غير معرفة',
      priceChanged: 'تغير في السعر',
      total: "إجمالي حالات الانتباه",
      session: "جلسة الجرد",
      selectSession: "اختر جلسة الجرد",
      attentionType: "نوع الحالة",
    },

    comparisonResults: {
      description: "مقارنة بين الجرد السابق والجرد الحالي",
      totalItems: "إجمالي العناصر",
      branch: "الفرع",
      store: "المستودع",
      inventoryType: "نوع الجرد",
      loadError: "فشل تحميل نتائج المقارنة",
      noActiveSession: "لا توجد جلسة جرد نشطة",
      title: "نتائج المقارنة",
      items: "عنصر",

      filters: "الفلاتر",
      clear: "مسح",
      status: "الحالة",
      search: "البحث (رقم الصنف )",
      searchPlaceholder: "بحث...",
      category: "التصنيف",
      unit: "الوحدة",
      all: "الكل",
      applyFilters: "تطبيق الفلاتر",

      itemCode: "رقم الصنف",
      itemName: "اسم الصنف",
      quantityBefore: "الكمية قبل الجرد",
      quantityAfter: "الكمية بعد الجرد",
      quantityDifference: "فرق الكمية",
      differencePercentage: "نسبة الفرق",
      priceBefore: "السعر قبل الجرد",
      priceAfter: "السعر بعد الجرد",
      beforeValue: "القيمة قبل الجرد",
      afterValue: "القيمة بعد الجرد",
      valueDifference: "فرق القيمة",
      status: "الحالة",

      loading: "جاري تحميل البيانات...",
      noResults: "لا توجد نتائج",

      of: "من",
      previous: "السابق",
      next: "التالي",

      statuses: {
        increase: "زيادة",
        decrease: "نقص",
        noDifference: "لا يوجد فرق",
        afterOnly: "موجود بعد الجرد فقط",
        beforeOnly: "موجود قبل الجرد فقط",
      },
    },
  },
  footer: {
    systemName: 'نظام إدارة المخزون',
    systemDescription: 'ذكي • موثوق • فعّال',
    copyright: 'Fatema Majid 2026',
    version: 'الإصدار 1.0.0',
    allRightsReserved: 'جميع الحقوق محفوظة',
  },
  inventorySessions: {
    title: "جلسات الجرد",
    description: "عرض وإدارة جلسات الجرد.",

    totalSessions: "مجموع الجلسات",
    activeSessions: "الجلسات النشطة",
    completedSessions: "الجلسات المكتملة",

    searchPlaceholder: "البحث في جلسات الجرد...",

    status: "الحالة",
    branch: "الفرع",
    store: "المستودع",

    allStatuses: "كل الحالات",
    allBranches: "كل الفروع",
    allStores: "كل المستودعات",

    active: "نشطة",
    completed: "مكتملة",
    inProgress: "قيد التنفيذ",
    cancelled: "ملغاة",

    clearFilters: "مسح",

    sessionsList: "جلسات الجرد",
    sessionsListDescription: "أحدث جلسات الجرد.",

    sessionId: "رقم الجلسة",
    date: "التاريخ",
    items: "الأصناف",
    actions: "الإجراءات",

    view: "عرض",
    loading: "جاري التحميل...",
    noSessions: "لا توجد جلسات جرد.",
    loadError: "فشل تحميل جلسات الجرد.",
    noOptions: "لا توجد خيارات",
  },

  pagination: {
    showing: "عرض",
    of: "من",
    rowsPerPage: "عدد الصفوف",
    previous: "السابق",
    next: "التالي",
  },

  reports: {
    title: "تقرير الجرد",
    description: "مراجعة وتحليل وتصدير نتائج جلسة الجرد المحددة.",
    session: "جلسة الجرد",
    date: "تاريخ الجرد",
    branch: "الفرع",
    store: "المخزن",
    noActiveSession: "اختر جلسة جرد لعرض تقريرها.",
    error: "حدث خطأ أثناء تحميل التقرير.",
    exportError: "حدث خطأ أثناء تصدير التقرير.",

    summary: {
      totalItems: "إجمالي الأصناف",
      match: "الأصناف المتطابقة",
      differences: "الأصناف ذات الفروقات",
      attention: "عناصر الانتباه",
    },

    financial: {
      before: "القيمة قبل الجرد",
      after: "القيمة بعد الجرد",
      difference: "فرق القيمة",
      percentage: "نسبة الفرق",
    },

    tabs: {
      overview: "نظرة عامة",
      comparison: "المقارنة",
    },

    actions: {
      excel: "تصدير Excel",
      pdf: "تصدير PDF",
      print: "طباعة",
    },

    statusOverview: {
      title: "حالة الجرد",
      description: "توزيع نتائج الأصناف التي تم جردها.",
    },

    statuses: {
      Increase: "زيادة",
      Decrease: "نقصان",
      Match: "متطابق",
      NewlyCounted: "تم عدّه حديثًا",
      FullyDepleted: "نفد بالكامل",
    },

    topDifferences: {
      title: "أكبر فروقات القيمة",
      description: "الأصناف ذات أكبر تغير في القيمة.",
      empty: "لا توجد فروقات في القيمة.",
    },
  },

  attentionItems: {
    title: "عناصر الانتباه",
    description: "العناصر التي تحتاج إلى مراجعة أو متابعة.",
    session: "جلسة الجرد",
    sessionDate: "تاريخ الجرد",
    branch: "الفرع",
    store: "المتجر",
    inventoryType: "نوع الجرد",

    summary: {
      total: "إجمالي عناصر الانتباه",
      newlyCounted: "تم عدّها حديثًا",
      fullyDepleted: "نفدت بالكامل",
      unitNotDefined: "الوحدة غير محددة",
      priceChanged: "تغيّر السعر",
    },

    filters: {
      search: "بحث بكود الصنف",
      attentionType: "نوع الانتباه",
      category: "التصنيف",
      unit: "الوحدة",
      all: "الكل",
      clear: "مسح",
    },

    types: {
      newlyCounted: "تم عدّها حديثًا",
      fullyDepleted: "نفدت بالكامل",
      unitNotDefined: "الوحدة غير محددة",
      priceChanged: "تغيّر السعر",
    },

    table: {
      itemCode: "كود الصنف",
      itemName: "اسم الصنف",
      category: "التصنيف",
      unit: "الوحدة",
      quantityBefore: "الكمية السابقة",
      quantityAfter: "الكمية الحالية",
      difference: "الفرق",
      consumerPriceBefore: "سعر المستهلك السابق",
      consumerPriceAfter: "سعر المستهلك الحالي",
      valueBefore: "القيمة السابقة",
      valueAfter: "القيمة الحالية",
      valueDifference: "فرق القيمة",
      attentionType: "نوع الانتباه",
    },

    empty: "لا توجد عناصر تحتاج إلى انتباه.",
    loading: "جاري تحميل البيانات...",
    error: "حدث خطأ أثناء تحميل عناصر الانتباه.",
  },

};

export default ar;