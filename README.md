# DeezLang

## Description

A low level compiled stack based language. DeezLang compiles to MASM32 assembly, which can then be compiled to a native executable.

## Installation

 - Create a new C# console application.
 - Paste all the scripts above.
 - Compile the C# program into a native executabe.

## Usage

```
DeezLang.exe [INPUT_FILE] [OUTPUT_FILE]
```

## Credits

 - Me (Development)
 - Tsoding (Idea)

## License

MIT License

Copyright (c) 2023 4xvoid

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Features

DeezLang has a total of 11 keyword, and 5 operators. Some of these include the `macro` keyword, which allows you to write a macro in assembly such as:

```
# StdOut macro
macro stdout
  "call StdOut"
end
```

## To Be Implemented

DeezLang is currently in development and I hope to add a TON of new features such as:

 - Procedures/Functions
 - A `goto` Keyword With Labels
 - A port of the standard C libary.
 - Structers/Classes

Keep in mind DeezLang was created in 3 days for fun, as I am only 13 years old.
