// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Stride.Core.MicroThreading;

/// <summary>
/// Provides microthread-local storage of data.
/// </summary>
/// <typeparam name="T">Type of data stored.</typeparam>
public class MicroThreadLocal<T> where T : class
{
    private readonly Func<T>? valueFactory;
    private readonly ConditionalWeakTable<MicroThread, T?> values = [];

    /// <summary>
    /// The value return when we are not in a micro thread. That is the value return when 'Scheduler.CurrentMicroThread==null'
    /// </summary>
    private T? valueOutOfMicrothread;

    /// <summary>
    /// Indicate if the value out of micro-thread have been set at least once or not.
    /// </summary>
    private bool valueOutOfMicrothreadSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroThreadLocal{T}"/> class.
    /// </summary>
    public MicroThreadLocal()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroThreadLocal{T}"/> class.
    /// </summary>
    /// <param name="valueFactory">The value factory invoked to create a value when <see cref="Value"/> is retrieved before having been previously initialized.</param>
    public MicroThreadLocal(Func<T>? valueFactory)
    {
        this.valueFactory = valueFactory;
    }

    /// <summary>
    /// Gets or sets the value for the current microthread.
    /// </summary>
    /// <value>
    /// The value for the current microthread.
    /// </value>
    public T? Value
    {
        get
        {
            T? value;
            var microThread = Scheduler.CurrentMicroThread;

            lock (values)
            {
                if (microThread == null)
                {
                    if (!valueOutOfMicrothreadSet)
                        valueOutOfMicrothread = valueFactory?.Invoke();
                    value = valueOutOfMicrothread;
                }
                else if (!values.TryGetValue(microThread, out value))
                {
                    values.Add(microThread, value = valueFactory?.Invoke());
                }
            }

            return value;
        }
        set
        {
            var microThread = Scheduler.CurrentMicroThread;

            lock (values)
            {
                if (microThread == null)
                {
                    valueOutOfMicrothread = value;
                    valueOutOfMicrothreadSet = true;
                }
                else
                {
                    values.Remove(microThread);
                    values.Add(microThread, value);
                }
            }
        }
    }

    public bool IsValueCreated
    {
        get
        {
            var microThread = Scheduler.CurrentMicroThread;

            lock (values)
            {
                if (microThread == null)
                    return valueOutOfMicrothreadSet;

                return values.TryGetValue(microThread, out var value);
            }
        }
    }

    public void ClearValue()
    {
        var microThread = Scheduler.CurrentMicroThread;

        lock (values)
        {
            if (microThread == null)
            {
                valueOutOfMicrothread = default;
                valueOutOfMicrothreadSet = false;
            }
            else
            {
                values.Remove(microThread);
            }
        }
    }
}
