# Fibonacci Service

It seems that the point of the **Fibonacci service** is to mimic **multiple slow operations** on external resource. To deal with this scenario, we need to write code that doesn't block our main program from running continuously. In other words, we can't wait for each operation to finish to start the next one; it's way better to perform these operations in parallel to save time.

> [!TIP]
> [Concurrency](https://en.wikipedia.org/wiki/Concurrency_(computer_science)) refers to the ability of an application to run more than one operation at the same time (concurrently), avoiding the block of the **main thread**. It allows multiple tasks to progress independently, leading to better performance.
> - The **main advantage** of concurrency is significantly reduced execution time, which results in a more performant program
> - The **main disadvantage** of concurrency is an increment in the use or resources, which can lead to system overloads (hence the requirement of limiting the **amount of memory** used by our program).

In **.NET** there are two ways of achieving concurrency:

- [Tasks](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0)
- [Threads](https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-8.0)

## Threads

Compare to tasks, **treads** are more **low level** and provide more **precise control** over parallel processing. With threads we are in charge of:

- Creating them.
- Starting and stopping them.
- Synchronizing them, preventing conflicts that may arise when multiple threads access shared resources. Also we're in charge of avoiding **deadlocks**, where threads are stuck waiting for each other to proceed.

## Tasks

Compared to traditional threads, **tasks** offer several advantages:

- **Lightweight**, they are less resource-intensive than threads, reducing overhead and improving performance.
- **Managed Execution**: Tasks are managed by the runtime environment, which handles resource allocation, scheduling, and synchronization.
- **Exception Handling**: Tasks provide a structured approach to exception handling in asynchronous code.

## My Solution

Since I don't have much experience with **concurrency**, and I heard that using **threads** is easy to shoot yourself in the foot, I'm gonna go with **tasks** in my solution.

> That being said, can't wait to start digging deeper into how to **master threads**.

Things that I use in my concurrent code:

- In the `timeoutTask` I set up a timer with the `timeout` received as argument. When this task finishes, I **cancel** the operation, and return a partial subsequence.

- The [lock](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock) statement, so that when a task is writing to the `subsequence` list, the resource is locked until the writing is done.

- The [CancellationTokenSource](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource?view=net-8.0) class, which is used to cancel the operation if the `timeoutTask` finishes before we have returned from the `GenerateSubsequence` method. If that's the case, I'm returning a partial `Subsequence` and the `TimeoutOcurred` flag.

  The line `await Task.Delay(500, cts.Token);` is key here

## The Binet's Formula

The [Binet's formula](https://en.wikipedia.org/wiki/Fibonacci_sequence#Binet's_formula) which allows to compute a random item in a Fibonacci sequence, without having to start from the beginning!