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
    english: "English",
    arabic: "العربية",
    changeLanguageToEnglish: "تغيير اللغة إلى الإنجليزية",
    changeLanguageToArabic: "تغيير اللغة إلى العربية",
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
      increase: 'الزيادة بالكمية بعد الجرد',
      decrease: 'نقص بالكمية بعد الجرد',
      match: 'الأصناف المطابقة',
      newlyCounted: 'أصناف جديدة بعد الجرد',
      fullyDepleted: 'أصناف ذات الرصيد 0 بعد الجرد',
      priceChanged: 'تغيير في سعر المستهلك',
      unitNotDefined: 'الوحدة الأولى غير معرفة',
      totalQuantityBefore: 'إجمالي الكميات قبل الجرد',
      totalQuantityAfter: 'إجمالي الكميات بعد الجرد',
      title: 'حالة العناصر',
      total: 'المجموع',

      topDifferences: {
        title: 'أعلى 10 مراكز حسب فرق القيمة (د.ع)',
        itemName: 'اسم الصنف',
      },
    },
    emptyState: {
      title: "لا توجد جلسة جرد محددة",
      description: "يرجى اختيار جلسة جرد لعرض بيانات لوحة التحكم.",
      selectSession: "اختيار جلسة الجرد",
    },
    errors: {
      loadDashboard: "فشل تحميل بيانات لوحة التحكم.",
      loadComparison: "فشل تحميل نتائج المقارنة.",
      loadAttention: "فشل تحميل عناصر الانتباه.",
      exportExcel: "فشل تصدير تقرير Excel.",
      exportPdf: "فشل تصدير تقرير PDF.",
    },

    inventoryQuantity: {
      title: 'ملخص الكميات',
      totalQuantityBefore: 'إجمالي الكميات قبل الجرد',
      totalQuantityAfter: 'إجمالي الكميات بعد الجرد',
      quantityDifference: 'فرق الكمية',
      quantityIncrease: 'الزيادة بالكمية',
      quantityDecrease: 'النقص بالكمية',
    },

    financialSummary: {
      title: "ملخص مالي",
      totalValueBefore: "إجمالي القيمة قبل الجرد",
      totalValueAfter: "إجمالي القيمة بعد الجرد",
      totalDifference: "الفرق بالقيمة",
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
      newlyCounted: 'أصناف جديدة بعد الجرد',
      fullyDepleted: 'أصناف ذات الرصيد 0 بعد الجرد',
      unitNotDefined: 'الوحدة الأولى غير معرفة',
      priceChanged: 'تغيير في سعر المستهلك',
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

  stores: {
    description: "إدارة المخازن والفروع وحالات المخازن.",
    newStore: "إضافة مخزن",
    editStore: "تعديل المخزن",
    totalStores: "إجمالي المخازن",
    activeStores: "المخازن النشطة",
    inactiveStores: "المخازن غير النشطة",
    searchPlaceholder: "البحث بالكود أو الاسم أو الفرع...",
    branch: "الفرع",
    allBranches: "كل الفروع",
    status: "الحالة",
    allStatuses: "كل الحالات",
    active: "نشط",
    inactive: "غير نشط",
    reset: "إعادة ضبط",
    storesList: "قائمة المخازن",
    storesListDescription: "عرض وإدارة جميع المخازن.",
    code: "كود المخزن",
    name: "اسم المخزن",
    actions: "الإجراءات",
    edit: "تعديل",
    delete: "حذف",
    noStores: "لا توجد مخازن.",
    loading: "جاري تحميل المخازن...",
    loadError: "فشل تحميل المخازن.",
    saveError: "فشل حفظ المخزن.",
    deleteError: "فشل حذف المخزن.",
    deleteConfirmation: "هل أنت متأكد من حذف {name}؟",
    editDescription: "حدّث معلومات المخزن أدناه.",
    arabicName: "الاسم العربي",
    englishName: "الاسم الإنكليزي",
    selectBranch: "اختر الفرع",
    formDescription: "أدخل معلومات المخزن أدناه.",
    storeCode: "كود المخزن",
    storeNameArabic: "الاسم العربي",
    storeNameEnglish: "الاسم الإنكليزي",
    branchCode: "الفرع",
    close: "إغلاق",
    cancel: "إلغاء",
    save: "حفظ",
    saving: "جاري الحفظ...",
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

  branches: {
    description: "إدارة الفروع ومعلومات الاتصال الخاصة بها.",
    addBranch: "إضافة فرع",
    editBranch: "تعديل الفرع",
    totalBranches: "إجمالي الفروع",
    activeBranches: "الفروع النشطة",
    inactiveBranches: "الفروع غير النشطة",
    searchPlaceholder: "البحث بالكود أو اسم الفرع...",
    status: "الحالة",
    allStatuses: "كل الحالات",
    active: "نشط",
    inactive: "غير نشط",
    clear: "مسح",
    listTitle: "قائمة الفروع",
    listDescription: "عرض وإدارة جميع الفروع.",
    code: "كود الفرع",
    arabicName: "الاسم العربي",
    englishName: "الاسم الإنكليزي",
    address: "العنوان",
    phone: "رقم الهاتف",
    actions: "الإجراءات",
    edit: "تعديل",
    delete: "حذف",
    empty: "لا توجد فروع.",
    loading: "جاري تحميل الفروع...",
    loadError: "فشل تحميل الفروع.",
    loadDetailsError: "فشل تحميل تفاصيل الفرع.",
    saveError: "فشل حفظ الفرع.",
    deleteError: "فشل حذف الفرع.",
    deleteConfirmation: "هل أنت متأكد من حذف {name}؟",
    formDescription: "أدخل معلومات الفرع أدناه.",
    close: "إغلاق",
    cancel: "إلغاء",
    save: "حفظ",
    saving: "جاري الحفظ...",
  },
  users: {
    title: "المستخدمون",
    description: "إدارة المستخدمين والصلاحيات",
    addUser: "إضافة مستخدم",
    editUser: "تعديل المستخدم",
    addUserDescription: "إنشاء مستخدم جديد وتحديد صلاحياته",
    editUserDescription: "تعديل بيانات المستخدم وصلاحياته",
    username: "اسم المستخدم",
    usernamePlaceholder: "أدخل اسم المستخدم",
    password: "كلمة المرور",
    passwordPlaceholder: "أدخل كلمة المرور",
    passwordEditPlaceholder: "اتركها فارغة لعدم تغيير كلمة المرور",
    role: "الدور",
    selectRole: "اختر الدور",
    activeAccount: "الحساب فعال",
    permissions: "الصلاحيات",
    permissionsDescription: "حدد الصلاحيات المباشرة لهذا المستخدم",
    save: "حفظ",
    saving: "جاري الحفظ...",
    cancel: "إلغاء",
    close: "إغلاق",
    edit: "تعديل",
    delete: "حذف",
    deactivate: "تعطيل",
    activate: "تفعيل",
    active: "فعال",
    inactive: "غير فعال",
    totalUsers: "إجمالي المستخدمين",
    activeUsers: "المستخدمون الفعالون",
    inactiveUsers: "المستخدمون غير الفعالين",
    searchPlaceholder: "البحث عن مستخدم...",
    allStatuses: "كل الحالات",
    noUsers: "لا يوجد مستخدمون",
    loading: "جاري تحميل المستخدمين...",
    actions: "الإجراءات",
    confirmDelete: "هل أنت متأكد من حذف هذا المستخدم؟",
    confirmDeactivate: "هل أنت متأكد من تعطيل هذا المستخدم؟",
    confirmActivate: "هل تريد تفعيل هذا المستخدم؟",
    createdAt: "تاريخ الإنشاء",
    updatedAt: "آخر تحديث",
    permissionsCount: "عدد الصلاحيات",
    successCreate: "تم إنشاء المستخدم بنجاح",
    successUpdate: "تم تحديث المستخدم بنجاح",
    successDelete: "تم حذف المستخدم بنجاح",
    errorLoad: "تعذر تحميل المستخدمين",
    errorSave: "تعذر حفظ بيانات المستخدم",
    errorDelete: "تعذر حذف المستخدم",
    status: "الحالة",
    notAvailable: "غير متوفر",
    allRoles: "كل الادوار",
  },

  login: {
    title: "نظام إدارة المخزون",
    description: "تسجيل الدخول للمتابعة",
    username: "اسم المستخدم",
    usernamePlaceholder: "أدخل اسم المستخدم",
    password: "كلمة المرور",
    passwordPlaceholder: "أدخل كلمة المرور",
    signIn: "تسجيل الدخول",
    signingIn: "جاري تسجيل الدخول...",
    requiredFields: "يرجى إدخال اسم المستخدم وكلمة المرور.",
    loginError: "اسم المستخدم أو كلمة المرور غير صحيحة.",
    invalidCredentials: "اسم المستخدم أو كلمة المرور غير صحيحة.",
    unauthorized: "ليس لديك صلاحية لتسجيل الدخول.",
    tooManyAttempts: "تم إجراء محاولات تسجيل دخول كثيرة. يرجى المحاولة لاحقًا.",
  },

  rolesPermissions: {
    title: "الأدوار والصلاحيات",
    description: "إدارة أدوار المستخدمين والصلاحيات المرتبطة بها.",
    addRole: "إضافة دور",
    editRole: "تعديل الدور",
    roles: "الأدوار",
    rolesDescription: "عرض وإدارة أدوار النظام.",
    totalRoles: "إجمالي الأدوار",
    totalPermissions: "إجمالي الصلاحيات",
    permissions: "الصلاحيات",
    permissionsDescription: "الصلاحيات المرتبطة بهذا الدور.",
    users: "المستخدمون",
    selectRoleTitle: "اختر دوراً",
    selectRoleDescription: "اختر دوراً من القائمة لعرض الصلاحيات المرتبطة به.",
    roleName: "اسم الدور",
    roleDescription: "وصف الدور",
    formDescription: "أدخل معلومات الدور وحدد الصلاحيات المطلوبة.",
    close: "إغلاق",
    cancel: "إلغاء",
    save: "حفظ",
    saving: "جاري الحفظ...",
    retry: "إعادة المحاولة",
    loadError: "فشل تحميل الأدوار والصلاحيات.",
    loadDetailsError: "فشل تحميل تفاصيل الدور.",
    saveError: "فشل حفظ الدور.",
    permissionGroups: {
      Dashboard: "لوحة التحكم",
      InventorySession: "جلسات الجرد",
      Attention: "العناصر التي تحتاج انتباه",
      Comparison: "المقارنة",
      Report: "التقارير",
      Branch: "الفروع",
      Store: "المخازن",
      User: "المستخدمون",
      Role: "الأدوار والصلاحيات",
      AuditLog: "سجل العمليات",
      Settings: "الإعدادات",
    },
    permissionNames: {
      "Dashboard.View": "عرض لوحة التحكم",
      "Dashboard.Export": "تصدير لوحة التحكم",
      "InventorySession.View": "عرض جلسات الجرد",
      "InventorySession.Create": "إنشاء جلسة جرد",
      "Attention.View": "عرض عناصر الانتباه",
      "Attention.Export": "تصدير عناصر الانتباه",
      "Comparison.View": "عرض نتائج المقارنة",
      "Comparison.Export": "تصدير نتائج المقارنة",
      "Report.View": "عرض التقارير",
      "Report.Create": "إنشاء التقارير",
      "Report.Export": "تصدير التقارير",
      "Branch.View": "عرض الفروع",
      "Branch.Create": "إنشاء فرع",
      "Branch.Edit": "تعديل الفرع",
      "Store.View": "عرض المخازن",
      "Store.Create": "إنشاء مخزن",
      "Store.Edit": "تعديل المخزن",
      "User.View": "عرض المستخدمين",
      "User.Create": "إنشاء مستخدم",
      "User.Edit": "تعديل المستخدم",
      "User.Deactivate": "تعطيل المستخدم",
      "Role.View": "عرض الأدوار والصلاحيات",
      "Role.Edit": "تعديل الأدوار والصلاحيات",
      "AuditLog.View": "عرض سجل العمليات",
      "Settings.View": "عرض الإعدادات",
    },
    roleNames: {
      Manager: "مدير",
      Admin: "مسؤول",
      User: "مستخدم",
    },
    roleDescriptions: {
      Manager: "صلاحيات كاملة لإدارة النظام.",
      Admin: "إدارة المخزون والمستخدمين.",
      User: "صلاحيات عرض فقط.",
    },
  },

  auditLog: {
    title: "سجل العمليات",
    description: "عرض ومتابعة جميع العمليات التي تمت على النظام.",
    stats: {
      total: "إجمالي العمليات",
      today: "عمليات اليوم",
      success: "العمليات الناجحة",
      failed: "العمليات الفاشلة"
    },
    filters: {
      title: "تصفية العمليات",
      reset: "إعادة تعيين",
      user: "المستخدم",
      action: "العملية",
      entity: "العنصر",
      dateFrom: "من تاريخ",
      dateTo: "إلى تاريخ",
      all: "الكل"
    },
    table: {
      title: "سجل العمليات",
      date: "التاريخ",
      user: "المستخدم",
      action: "العملية",
      entity: "العنصر",
      status: "الحالة",
      details: "التفاصيل",
      empty: "لا توجد عمليات",
      loading: "جاري التحميل..."
    },
    details: {
      title: "تفاصيل العملية",
      close: "إغلاق",
      entityId: "معرف العنصر",
      details: "التفاصيل",
      name: "الاسم",
      code: "الكود",
      branchCode: "كود الفرع"
    },
    actions: {
      Create: "إنشاء",
      Update: "تعديل",
      Delete: "حذف",
      Confirm: "تأكيد",
      Login: "تسجيل دخول",
      LoginFailed: "فشل تسجيل الدخول"
    },
    entities: {
      Branch: "فرع",
      Store: "مخزن",
      User: "مستخدم",
      Role: "دور",
      InventorySession: "جلسة جرد"
    },
    status: {
      Success: "ناجحة",
      Failed: "فاشلة"
    }
  },
};

export default ar;