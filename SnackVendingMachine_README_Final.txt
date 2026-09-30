SNACK VENDING MACHINE
=====================

C# / .NET OOP & Data Structures Training Task

OVERVIEW
--------
This project implements the backend logic of an automated Snack Vending Machine.

The project focuses on:
- Clean Object-Oriented design
- Appropriate data structures
- State-based transaction control
- Payment and change handling
- Hardware abstraction
- Exception handling
- Automated unit testing

The vending machine is modeled as a 5 x 5 grid with 25 selectable slot positions.

MAIN DESIGN
-----------
1. SnackMachine
   - Coordinates the complete purchase workflow
   - Selects items
   - Accepts payments
   - Completes or cancels purchases
   - Controls state transitions
   - Uses display and dispenser abstractions

2. Inventory
   - Manages all vending machine slots
   - Uses Dictionary<int, Slot>
   - Slot code is used as the dictionary key

3. Slot
   - Represents one physical vending-machine position
   - Contains Stack<Snack>
   - The stack represents the depth of products in the slot
   - Stock is derived from the stack count

4. Snack
   - Represents a snack item
   - Contains Name and PriceCents
   - Price is stored in cents using long

5. Payment
   - Represents a customer payment
   - Validates payment type and denomination

6. CashVault
   - Tracks physical money inside the machine
   - Uses SortedDictionary<decimal, int>
   - Stores denomination -> available quantity
   - Checks and returns exact change safely

7. State Pattern
   - IMachineState
   - IdleState
   - PaymentState
   - DispensingState

8. Hardware Abstractions
   - IDisplay
   - IDispenser
   Current implementations:
   - ConsoleDisplay
   - ConsoleDispenser

9. VendingException
   - Custom exception for vending-machine domain errors

5 x 5 INVENTORY MODEL
---------------------
The machine has 25 selectable positions:

11  12  13  14  15
21  22  23  24  25
31  32  33  34  35
41  42  43  44  45
51  52  53  54  55

Each position represents one Slot.

Each Slot contains a stack of Snack objects:

Slot
 |
 +-- Snack   <- next item to dispense
 +-- Snack
 +-- Snack
 +-- Snack

The stack follows LIFO:
Last In, First Out.

Main stack operations:
- Push  -> add a snack
- Peek  -> inspect the next snack without removing it
- Pop   -> dispense one snack

WHY DICTIONARY FOR INVENTORY?
-----------------------------
Inventory uses:

Dictionary<int, Slot>

Example:

11 -> Slot
12 -> Slot
13 -> Slot

This allows direct lookup using the numeric keypad code.

PAYMENT RULES
-------------
Currency:
USD only

Accepted coins:
- $0.10
- $0.20
- $0.50
- $1.00

Accepted notes:
- $20.00
- $50.00

Card:
- Supported for exact transaction settlement

PURCHASE FLOW
-------------
1. Customer selects a slot code
2. Machine verifies that the slot exists
3. Machine verifies that stock is available
4. Machine enters PaymentState
5. Customer inserts payment
6. Payment is validated
7. Current balance is updated
8. Machine verifies that payment is sufficient
9. Required change is calculated
10. Machine verifies that exact change can be produced
11. Machine enters DispensingState
12. One snack is removed from the slot
13. IDispenser is called
14. Required change is removed from CashVault
15. Transaction data is reset
16. Machine returns to IdleState

STATE PATTERN
-------------
IdleState
   |
   | Select Item
   v
PaymentState
   |
   | Complete Purchase
   v
DispensingState
   |
   | Finished
   v
IdleState

Cancellation from PaymentState returns the machine to IdleState.

TRANSACTION SAFETY
------------------
The purchase logic follows a prepare-before-commit approach.

Before modifying inventory or cash, the machine verifies:
- Valid selected slot
- Available stock
- Sufficient payment
- Exact change availability

Only after all checks succeed does the machine:
- Remove the snack
- Call the dispenser
- Deduct change
- Reset the transaction

This prevents partially completed transactions.

CHANGE CALCULATION
------------------
The machine first calculates:

Change = Inserted Money - Snack Price

CanMakeChange:
- Checks whether exact change can be created
- Does not modify the real CashVault

MakeChange:
- Finds a complete valid denomination combination
- Deducts money only after a valid solution is found

This prevents partial cash modification when exact change is impossible.

CANCELLATION AND REFUND
-----------------------
When an active purchase is cancelled:
- The transaction is stopped
- Inserted cash is refunded correctly
- Snack stock is not changed
- Machine returns to IdleState

HARDWARE ABSTRACTION
--------------------
SnackMachine does not depend directly on physical hardware.

IDisplay:
- Used for machine messages
- Current implementation: ConsoleDisplay

IDispenser:
- Used to represent the physical dispensing action
- Current implementation: ConsoleDispenser

These dependencies are injected into SnackMachine through its constructor.

DEPENDENCY INJECTION
--------------------
SnackMachine receives:
- Inventory
- CashVault
- IDisplay
- IDispenser

This improves:
- Testability
- Maintainability
- Separation of responsibilities
- Replaceability of hardware implementations

ERROR HANDLING
--------------
The project uses VendingException for domain errors.

Handled cases include:
- Invalid slot code
- Out of stock
- Insufficient funds
- Unsupported payment type
- Invalid denomination
- Exact change unavailable
- Invalid machine state

PROJECT STRUCTURE
-----------------
Task_1_OOP/
|
|-- SnackVendingMachine/
|   |
|   |-- Models/
|   |   |-- Snack.cs
|   |   `-- Slot.cs
|   |
|   |-- Inventory/
|   |   `-- Inventory.cs
|   |
|   |-- Payment/
|   |   |-- Payment.cs
|   |   `-- CashVault.cs
|   |
|   |-- States/
|   |   |-- IMachineState.cs
|   |   |-- IdleState.cs
|   |   |-- PaymentState.cs
|   |   `-- DispensingState.cs
|   |
|   |-- Hardware/
|   |   |-- IDisplay.cs
|   |   |-- ConsoleDisplay.cs
|   |   |-- IDispenser.cs
|   |   `-- ConsoleDispenser.cs
|   |
|   |-- Machine/
|   |   `-- SnackMachine.cs
|   |
|   |-- Exceptions/
|   |   `-- VendingException.cs
|   |
|   |-- Program.cs
|   `-- SnackVendingMachine.csproj
|
`-- SnackVendingMachine.Tests/
    |-- SlotTests.cs
    |-- InventoryTests.cs
    |-- PaymentTests.cs
    |-- CashVaultTests.cs
    |-- SnackMachineTests.cs
    `-- SnackVendingMachine.Tests.csproj

BUILD AND RUN
-------------
From the main project folder:

cd SnackVendingMachine
dotnet build
dotnet run

RUNNING TESTS
-------------
From the test project folder:

cd SnackVendingMachine.Tests
dotnet test

CURRENT VERIFICATION
--------------------
Build:
- 0 warnings
- 0 errors

Tests:
- Passed: 30
- Failed: 0
- Skipped: 0

TEST COVERAGE
-------------
The automated tests cover:
- Adding snacks
- Stack LIFO behavior
- Empty slot handling
- Invalid slot codes
- Restocking
- Invalid payment denominations
- Exact-payment purchase
- Purchase with change
- Insufficient funds
- Exact change unavailable
- Out-of-stock behavior
- Cancellation and refund
- State reset after successful purchase
- State reset after cancellation
- Dispenser called exactly once on successful sale
- Dispenser not called on failed sale

KEY DESIGN DECISIONS
--------------------
1. Stack<Snack>
   Represents the physical depth of snack items inside a slot.

2. Dictionary<int, Slot>
   Provides direct slot lookup using keypad codes.

3. SortedDictionary<decimal, int>
   Tracks cash denominations and available quantities.

4. State Pattern
   Separates behavior according to the current machine state.

5. Dependency Injection
   Keeps SnackMachine independent from concrete hardware implementations.

6. VendingException
   Represents vending-machine domain failures clearly.

7. Prepare-before-commit transaction logic
   Prevents partial inventory or cash updates.

FINAL STATUS
------------
Build:
SUCCESS
0 warnings
0 errors

Automated tests:
30 passed
0 failed
0 skipped
