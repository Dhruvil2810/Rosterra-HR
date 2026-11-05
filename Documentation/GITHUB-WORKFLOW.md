# Git & GitHub Team Workflow — Rosterra HR

This guide establishes the version control rules and commands for both team members on the Rosterra HR project.

---

## 1. Golden Rules of Git
1. **Never commit directly to `main` for new features.**
2. **Never commit build artifacts (`bin/`, `obj/`, `.vs/`, `*.user`).** Always use `.gitignore`.
3. **Always pull latest `main` before starting a new feature branch.**
4. **Make small, focused commits with clear messages.**
5. **Never commit database files (`.mdf`, `.ldf`).** Commit versioned SQL scripts in `Scripts/Database/`.

---

## 2. Branching Strategy
- `main`: Production-ready, stable codebase.
- `feature/<feature-name>`: Isolated branch for specific tasks (e.g., `feature/employee-crud`, `feature/auth`).

---

## 3. Essential Git Commands Cheat Sheet

### Starting Work on a New Task
```powershell
# 1. Switch to main and get the latest updates
git switch main
git pull origin main

# 2. Create and switch to your feature branch
git switch -c feature/employee-model
```

### Checking Changes & Staging
```powershell
# Check which files have been modified or added
git status

# View the exact line-by-line differences
git diff

# Stage specific files (never do 'git add .' blindly)
git add ROSTERRA HR/Models/Employee.cs
```

### Committing & Pushing
```powershell
# Commit with a clear, imperative message
git commit -m "Add Employee domain model with validation attributes"

# Push the branch to GitHub for the first time
git push -u origin feature/employee-model
```

### Merging Feature Branch into Main
Once tested and approved:
```powershell
git switch main
git pull origin main
git merge feature/employee-model
git push origin main
```

---

## 4. Handling Teammate Code via WhatsApp/External Files
If your teammate shares a standalone `.cs` or `.sql` file over WhatsApp:
1. Verify what file it is, its intended namespace, and its dependencies.
2. Create an isolated branch: `git switch -c teammate-integration/department-model`.
3. Place the file in its proper folder (e.g., `ROSTERRA HR/Models/`).
4. Build in Visual Studio 2022 to verify compilation.
5. Review the code for security (parameterized queries, input validation).
6. Stage and commit:
   ```powershell
   git add "ROSTERRA HR/Models/Department.cs"
   git commit -m "Integrate Department model contributed by teammate"
   ```
7. Merge into `main` only after full build verification.
