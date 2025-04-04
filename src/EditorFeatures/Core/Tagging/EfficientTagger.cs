﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.CodeAnalysis.Editor.Tagging;

/// <summary>
/// Root type of all roslyn <see cref="ITagger{T}"/> implementations.  This adds specialized hooks that allow for tags
/// to be added to a preexisting list, rather than generating a fresh list instance.  This can help with avoiding
/// garbage by allowing us to pool intermediary lists (especially as some high-level aggregating taggers defer to
/// intermediary taggers).
/// </summary>
internal abstract class EfficientTagger<TTag> : ITagger<TTag>, IDisposable where TTag : ITag
{
    /// <summary>
    /// Produce the set of tags with the requested <paramref name="spans"/>, adding those tags to <paramref name="tags"/>.
    /// </summary>
    public abstract void AddTags(NormalizedSnapshotSpanCollection spans, SegmentedList<TagSpan<TTag>> tags);

    public abstract void Dispose();

    IEnumerable<ITagSpan<TTag>> ITagger<TTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        => GetTags(spans);

    /// <summary>
    /// Default impl of the core <see cref="ITagger{T}"/> interface.
    /// </summary>
    public IEnumerable<TagSpan<TTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
        using var _ = SegmentedListPool.GetPooledList<TagSpan<TTag>>(out var list);

        AddTags(spans, list);

        // Use yield return mechanism to allow the segmented list to get returned back to the
        // pool after usage. This does cause an allocation for the yield state machinery, but
        // that is better than not freeing a potentially large segmented list back to the pool.
        foreach (var item in list)
            yield return item;
    }

    public virtual event EventHandler<SnapshotSpanEventArgs>? TagsChanged;

    protected void OnTagsChanged(object? sender, SnapshotSpanEventArgs e)
        => TagsChanged?.Invoke(this, e);
}
