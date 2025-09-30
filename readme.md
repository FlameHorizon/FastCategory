# FastCategory

**FastCategory** is a web application designed to help users quickly and efficiently categorize and track their financial transactions. Built with ASP.NET Core, Blazor, and MudBlazor, it provides a modern, interactive user experience for managing personal or small business finances.

## Purpose

While using MMEX application I was struggling with entering data fast. As the whole, application
is fine, and does the job. But data entry I flet as it could be better. That is way I've made this project.
To speed up this process.

It is particularly useful for users who want to:

- **Quickly record expenses and income** with minimal friction.
- **Split transactions** into multiple categories (e.g., a single purchase split between groceries and household items).
- **Use keyboard shortcuts** for rapid data entry, making the app efficient for power users.
- **Export transactions** in QIF format for use with other financial software.

## Key Features

- **Intuitive Transaction Entry:**  
  Enter transaction details such as date, type, payee, and total amount with ease.

- **Category Splitting:**  
  Split a transaction into multiple categories and assign amounts to each. The UI supports adding/removing splits dynamically.

- **Keyboard Shortcuts:**  
  Power users can use shortcuts (e.g., `Alt+A` to add a split, `Alt+S` to save) for even faster data entry.

- **Autocomplete & Custom Categories:**  
  Category and payee fields support autocomplete from a configurable list, but also allow new entries.

- **Export to QIF:**  
  Download all entered transactions as a QIF file for import into other financial tools.

- **Responsive UI:**  
  Built with MudBlazor for a clean, responsive, and accessible interface.

## Who Is This For?

- Individuals who want a fast, keyboard-driven way to track expenses.
- Small business owners or freelancers needing a simple transaction log.
- Developers and testers looking for a Blazor/MudBlazor sample project with real-world features.

## How It Works

1. **Enter a transaction:**  
   Fill in the date, type, payee, and total amount.

2. **Split into categories:**  
   Add as many category splits as needed, assigning amounts to each.

3. **Save the transaction:**  
   Use the Save button or `Alt+S` shortcut.

4. **Export your data:**  
   Download your transactions as a QIF file for use elsewhere.

## Technical Highlights

- **Blazor Server** for interactive, real-time UI.
- **MudBlazor** for material design components.
- **In-memory storage** for simplicity (no database setup required).
- **AppSettings-driven configuration** for categories and payees.

---

**FastCategory** is ideal for anyone who values speed, flexibility, and simplicity in financial tracking.  
Feel free to explore, extend, or adapt it for your own needs!