# 🎨 FIGMA DIZAYN PROMPT - METRA v3.0

## PROJECT OVERVIEW

Create a professional desktop application UI design in Figma for "Metra v3.0" - a rental management system (equipment/materials rental).

### PROJECT DETAILS:
- **Application Name:** METRA v3.0
- **Type:** WPF Desktop Application (Windows)
- **Size:** 1366x768px (can be resized, responsive design)
- **Design Style:** Modern, Clean, Material Design inspired
- **Language:** Uzbek (O'zbek tili)

---

## COLOR SCHEME
**Strictly follow these colors:**

### Primary Colors:
- **Primary Purple:** `#4447E2` (main accent color for headers, primary buttons)
- **Forest Green:** `#228B22` (success actions, secondary accent)
- **Background:** `#F8F8F8` (main app background)
- **Card Background:** `#FFFFFF` (white cards with shadows)

### Text Colors:
- **Primary Text:** `#110C27` (dark, main text)
- **Secondary Text:** `#625F6E` (labels, hints)
- **Tertiary Text:** `#8A94A6` (disabled, placeholder)
- **Link/Accent Text:** `#4447E1` (clickable items)

### UI Element Colors:
- **TextBox Background:** `#EAEDF2` (light gray)
- **Table Header:** `#F3F2F7` (very light purple-gray)
- **Table Row:** `#FFFFFF` with hover `#F6F6F9`
- **Border:** `#E0E0E0` (light gray borders)
- **Danger/Delete:** `#FF0000` (red for delete actions)
- **Success:** `#00C853` (green for success)

---

## LAYOUT STRUCTURE

### 1. Top App Bar (60px height):
- **Left:** METRA logo icon + "METRA" text (24px, bold) + "v3.0" (12px, subtle)
- **Right:** Settings icon button, Logout icon button
- **Background:** `#4447E2` (purple), white text/icons
- **Shadow:** elevation 4dp

### 2. Sidebar Navigation (250px width):
- **Background:** white with subtle shadow
- **Rounded:** top-right and bottom-right corners (20px)
- **Expandable sections (Expanders):**
  * Ma'lumotlar (Data)
  * Hujjatlari (Documents)
  * Hisobotlar (Reports)
  * Ombor (Warehouse)
  * Xodimlar (Employees)
- **Active item:** purple background (`#4447E2`) with white text
- **Hover effect:** light gray background

### 3. Main Content Area (rest of the space):
- **Background:** `#F8F8F8`
- **Padding:** 20px
- **Contains:** dynamic page content

---

## KEY UI COMPONENTS TO DESIGN

### BUTTONS (create variations):

#### 1. Primary Button:
- Background: `#4447E2`, white text
- Padding: 12px 24px
- Border-radius: 8px
- Height: 40px
- Hover: darker shade `#3337C1`
- With icon + text option

#### 2. Secondary Button:
- Background: `#F3F2F7`, dark text `#625F6E`
- Same size as primary
- Hover: `#E5E4E9`

#### 3. Danger Button:
- Background: `#FF3D00`, white text
- For delete actions

#### 4. Outline Button:
- Border: 2px solid `#4447E2`
- Background: transparent
- Text: `#4447E2`

#### 5. Icon Button:
- Circular or square (40x40px)
- Icon only, no text
- Hover: light background

### TEXTBOXES/INPUTS:

#### 1. Standard TextBox:
- Background: `#EAEDF2`
- Border: 1px solid `#E0E0E0`
- Border-radius: 8px
- Height: 40px
- Padding: 10px 12px
- Placeholder text: `#8A94A6`
- Focus: purple border `#4447E2`

#### 2. Search Box:
- With magnifying glass icon on left or right
- Light background
- Rounded corners 20px
- Max-width: 400px

#### 3. Multiline TextBox:
- Same style as standard
- Variable height (min 100px)
- Scrollbar when needed

### CARDS/CONTAINERS:

#### 1. Card Border:
- Background: white
- Border-radius: 12px
- Shadow: `0 2px 8px rgba(0,0,0,0.1)`
- Padding: 20px

#### 2. Search Box Border:
- Background: white
- Border-radius: 20px
- Light shadow
- Padding: 8px 16px

### DATAGRID/TABLE:

#### Header row:
- Background: `#F3F2F7`
- Text: `#110C27`, semibold
- Height: 48px
- Border-bottom: 1px solid `#E0E0E0`

#### Data rows:
- Background: white
- Alternating: keep white, but hover shows `#F6F6F9`
- Height: 56px
- Border-bottom: 1px solid `#F3F2F7`

#### Selected row:
- Background: `#E8E8FF` (very light purple)
- Border-left: 4px solid `#4447E2`

#### Other details:
- Columns with proper spacing
- Right-aligned actions column (edit, delete icons)

### DIALOG/MODAL:

#### Overlay:
- Semi-transparent black (`#000000` with 50% opacity)

#### Dialog box:
- Width: 500px
- Background: white
- Border-radius: 12px
- Shadow: `0 8px 32px rgba(0,0,0,0.2)`

#### Dialog header:
- Background: `#4447E2`
- White text, 18px semibold
- Height: 60px
- Close button (X) on right
- Rounded top corners (12px)

#### Dialog content:
- Padding: 20px
- Scrollable if needed

#### Dialog footer:
- Border-top: 1px solid `#E0E0E0`
- Padding: 20px
- Buttons aligned right

---

## SPECIFIC PAGES TO DESIGN

### 1. WELCOME/HOME PAGE:
- Centered logo (128x128px, semi-transparent)
- "Metra Ijaraga Boshqaruv Tizimi" title (32px)
- "Versiya 3.0" subtitle
- "Clean Architecture + MVVM" tag
- Minimalist, elegant

### 2. FILIALLAR (BRANCHES) PAGE:

#### Top toolbar card:
- **Left:** "Yangi filial" button (green primary) + Refresh icon button
- **Right:** Search box

#### Main data table showing:
- \# (number column)
- Filial nomi (branch name)
- Turi (type: Asosiy/Filial/Ombor)
- Ta'rif (description)
- Mas'ul xodim (responsible worker)
- Yaratilgan sana (created date)

#### Row actions:
- Edit icon
- Delete icon

### 3. ADD/EDIT FILIAL DIALOG:

#### Title:
- "Yangi filial qo'shish" or "Filialni tahrirlash"

#### Form fields:
- **Filial nomi \*** (required textbox)
- **Filial turi \*** (dropdown: Asosiy, Filial, Ombor)
- **Ta'rif** (multiline textbox, optional)

#### Footer buttons:
- "Bekor qilish" (cancel, outline style)
- "Saqlash" (save, primary with icon)

---

## ICONS TO USE

**Icon Set:** Material Design Icons (sharp style)

### Common icons needed:
- Domain/Store (for branches)
- Plus (add)
- Pencil (edit)
- Delete/Trash
- Refresh
- Magnify/Search
- Settings
- Logout
- Close (X)
- Save/ContentSave

---

## TYPOGRAPHY

### Font Family:
- Use **"Roboto"** or similar modern sans-serif

### Font sizes:
- **Page title:** 24px, semibold
- **Section headers:** 18px, semibold
- **Body text:** 14px, regular
- **Small text/captions:** 12px, regular
- **Button text:** 14px, medium

---

## SPACING & RHYTHM

- **Grid system:** 8px
- **Card margins:** 20px
- **Component spacing:** 16px vertical, 12px horizontal
- **Button padding:** 12px vertical, 24px horizontal
- **Icon sizes:** 20px (small), 24px (medium), 32px (large)

---

## SHADOWS & DEPTH

- **Cards:** `0 2px 8px rgba(0,0,0,0.1)`
- **Buttons hover:** `0 4px 12px rgba(68,71,226,0.3)`
- **Dialogs:** `0 8px 32px rgba(0,0,0,0.2)`
- **Dropdown:** `0 4px 16px rgba(0,0,0,0.15)`

---

## STATES TO DESIGN

- ✅ Default state
- ✅ Hover state
- ✅ Active/Pressed state
- ✅ Disabled state
- ✅ Loading state (with spinner)
- ✅ Empty state (no data)
- ✅ Error state

---

## ADDITIONAL NOTES

- Design should feel modern, professional, and clean
- Uzbek text should be properly displayed
- All interactive elements should have clear hover/active states
- Use consistent spacing and alignment
- Icons should be aligned with text baseline
- Ensure good contrast ratios for accessibility
- Design for both light mode (primary)

---

## OUTPUT REQUIREMENTS

- ✅ Create component library with all reusable elements
- ✅ Design at least 3-5 key screens
- ✅ Include responsive behavior notes
- ✅ Export in Figma format
- ✅ Provide style guide document
- ✅ Create prototype with basic interactions (click navigation)

---

## AI TOOLS TO USE THIS PROMPT

You can use this prompt with the following AI design tools:

1. **Figma AI** - Built into Figma
2. **Uizard** - https://uizard.io
3. **Galileo AI** - https://usegalileo.ai
4. **Relume** - https://relume.io
5. **Visily** - https://visily.ai
6. **Mockitt** - https://mockitt.com

---

## TIPS FOR BEST RESULTS

- You can split the prompt into sections (e.g., first color scheme, then components)
- Create separate prompts for each page for more detailed results
- Iterate and refine based on the generated output
- Start with the component library, then move to full pages
- Request variations of components to choose the best design

---

**Generated for:** Metra v3.0 - Ijaraga Boshqaruv Tizimi
**Date:** 2025
**Version:** 1.0
