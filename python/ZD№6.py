import fitz  # PyMuPDF

input_pdf = "C:\projects\Education\python\split-document-2 (edited)-2.pdf"
output_pdf = "split-document-2 (edited)-2_updated.pdf"

# Открываем документ
doc = fitz.open(input_pdf)
page = doc[0]  # Работаем с первой страницей

# Ищем текст-ориентир "отчет по практической работе"
text_instances = page.search_for("отчет по практической работе")

if text_instances:
    # Берем координаты первой найденной строки
    rect = text_instances[0]
    # Вычисляем позицию: центр страницы по X, и на 35 пунктов выше найденного текста по Y
    y_pos = rect.y0 - 35
    x_pos = page.rect.width / 2
    
    # Вставляем ФИО
    page.insert_text(
        (x_pos, y_pos),
        "Сеитбекиров Руслан Эмилевич",
        fontsize=14,
        fontname="tiro",  # Встроенный шрифт с засечками, полный аналог Times New Roman
        align=fitz.TEXT_ALIGN_CENTER
    )
    print("✅ Текст успешно добавлен над нужной строкой!")
else:
    # Резервный вариант: если фраза не найдена, добавляем просто в верхнюю часть по центру
    page.insert_text(
        (page.rect.width / 2, 80),
        "Сеитбекиров Руслан Эмилевич",
        fontsize=14,
        fontname="tiro",
        align=fitz.TEXT_ALIGN_CENTER
    )
    print("⚠️ Текст-ориентир не найден, добавлено в верхнюю часть страницы.")

# Сохраняем результат
doc.save(output_pdf)
doc.close()
print(f"📄 Готово! Файл сохранен как: {output_pdf}")