
export function parseNumber(text) {
    if (!text) return 0;

    const persianDigits = {
        '۰': '0', '۱': '1', '۲': '2', '۳': '3', '۴': '4',
        '۵': '5', '۶': '6', '۷': '7', '۸': '8', '۹': '9',
        '٠': '0', '١': '1', '٢': '2', '٣': '3', '٤': '4',
        '٥': '5', '٦': '6', '٧': '7', '٨': '8', '٩': '9'
    };

    const clean = String(text)
        .replace(/[۰-۹٠-٩]/g, d => persianDigits[d]) // تبدیل ارقام فارسی/عربی
        .replace(/,/g, '')                            // حذف کاماها
        .replace(/[^\d.-]/g, '');                     // حذف سایر کاراکترها

    return Number(clean) || 0;
}

// --- فرمت عدد به رشته با جداکننده هزارگان ---
export function formatCurrency(value) {
    return Number(value || 0).toLocaleString('fa-IR'); // یا 'en-US' بسته به نیاز
}

// --- به‌روزرسانی مبلغ کل ---
export function updateTotal() {
    const subtotal = parseNumber($("#subtotalPrice").text());
    const discount = parseNumber($("#distotalPrice").text()); // اصلاح آیدی
    const delivery = parseNumber($("#deliveryPrice").text());

    const total = subtotal - discount + delivery;

    $("#sumPrice").text(formatCurrency(total)); // فرمت شده
}

