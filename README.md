# C# Calculator

A simple calculator application implemented in C#. Provides a minimal, reliable UI for basic arithmetic operations and is intended as a small desktop utility and learning project.

## Table of contents
- [Features](#features)
- [Requirements](#requirements)
- [Build & run](#build--run)
- [Usage](#usage)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features
- Basic arithmetic: add, subtract, multiply, divide
- Decimal numbers and standard operator precedence
- Simple keyboard support (digits and operators)
- Clear and backspace functionality
- Lightweight desktop UI (open the solution in Visual Studio to inspect the implementation)

## Requirements
- Visual Studio 2026 (or newer) for best development experience
- .NET SDK matching the project's `TargetFramework` (check the `*.csproj` file)

## Build & run
1. Clone the repository
    git clone https://github.com/Robesz0021/CSharpCalculator
    cd CSharpCalculator

2. Open the solution in Visual Studio by double-clicking the `*.sln` file or via __File > Open > Project/Solution__.

3. Build the solution using __Build > Build Solution__ and run:
   - Start debugging: __F5__ or __Debug > Start Debugging__
   - Start without debugging: __Ctrl+F5__ or __Debug > Start Without Debugging__

Alternatively, if the project is an SDK-style .NET project you can use the CLI:
    dotnet build
    dotnet run --project <path-to-project-file>

## Usage
- Enter numbers and choose operators using the UI or keyboard.
- Press `Enter` (or the equals button) to evaluate an expression.
- Use `Backspace` to remove the last digit and `Esc` (or the clear button) to clear the input.
- Example: enter `12 + 34` then press `Enter` to get `46`.

(Refer to the project UI and code for exact supported keys and behaviors.)

## Tests
If the repository contains unit tests, run them with:
    dotnet test

Or run tests from Visual Studio using the __Test Explorer__.

## Contributing
Contributions, issues and feature requests are welcome.
- Create a fork, open a branch, and submit a pull request.
- Use clear commit messages and include tests for new behaviors when possible.
- For discussion, open an issue describing the change or bug.

## License
See the `LICENSE` file in the repository for licensing details. If no license file exists, contact the repository owner to clarify reuse permissions.

## Contact
For questions about the project, open an issue in this repository or contact the repository owner via the associated GitHub account.