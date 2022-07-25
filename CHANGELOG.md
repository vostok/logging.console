## 1.0.9 (25-07-2022):

Used new `ConcurrentBoundedQueue.TryWaitForNewItemsBatchAsync` method with delay to avoid thread pool issues.

## 1.0.8 (06-12-2021):

Added `net6.0` target.

## 1.0.6 (24.08.2021):

Rewrite `SynchronousConsoleLog` (see [#5](https://github.com/vostok/logging.console/issues/5)).


## 1.0.5 (29.09.2020):

Optimized rendering of unstructured log events without actual templating in messages.

## 1.0.3 (25.06.2020):

Slight performance improvements.

## 1.0.2 (18.10.2019):

Fixed lowerCamelCase `WellKnownProperties`.

## 1.0.0 (11.03.2019):

Breaking change: ForContext() is now hierarchical.

## 0.1.3 (19.02.2019):

Added synchronous version of ConsoleLog called SynchronousConsoleLog.

## 0.1.2 (01.10.2018):

Fixed a small bug in output redirection detection.

## 0.1.1 (27.09.2018):

ConsoleLog now detects output redirection via Console.SetOut().

## 0.1.0 (06-09-2018): 

Initial prerelease.