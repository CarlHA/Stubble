﻿// <copyright file="SectionTests.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Stubble.Core.Classes;
using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Renderers;
using Stubble.Core.Dev.Renderers.Token;
using Stubble.Core.Dev.Settings;
using Stubble.Core.Dev.Tags;
using Xunit;

namespace Stubble.Core.Tests.Renderers.StringRenderer
{
    public class SectionTests : RendererTestsBase
    {
        [Fact]
        public void It_Can_Render_Section_Tags_AsCondition()
        {
            const string result = "Bar";

            var context = new Context(
                new
                {
                    condition = true,
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "condition",
                    Children = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = new StringSlice("bar") }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void ItIgnoresFalseySectionTags()
        {
            const string result = "";

            var context = new Context(
                new
                {
                    condition = false,
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "condition",
                    Children = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = new StringSlice("bar") }
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_IEnumerables_As_Lists()
        {
            const string result = "1 Bar\n2 Bar\n3 Bar\n4 Bar\n";

            var context = new Context(
                new
                {
                    list = new[]
                    {
                        new { a = 1 },
                        new { a = 2 },
                        new { a = 3 },
                        new { a = 4 }
                    },
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "list",
                    Children = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = new StringSlice("a") },
                        new LiteralTag { Content = new [] { new StringSlice(" ") } },
                        new InterpolationTag { Content = new StringSlice("bar") },
                        new LiteralTag { Content = new [] { new StringSlice("\n") } },
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_IEnumerators()
        {
            const string result = "a b c d e f g ";

            // Get Enumerator doesn't exist on string (netstandard 1.3)
            // will be added back in netstandard 2.0
            var enumerator = "abcdefg".ToCharArray().GetEnumerator();

            var context = new Context(
                new
                {
                    list = enumerator,
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "list",
                    Children = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = new StringSlice(".") },
                        new LiteralTag { Content = new [] { new StringSlice(" ") } },
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_LambdaTags_WithoutContext()
        {
            const string result = "1";

            var context = new Context(
                new
                {
                    lambda = new Func<string, object>((str) => 1),
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "lambda",
                    Children = new List<MustacheTag>
                    {
                        new InterpolationTag { Content = new StringSlice(".") },
                        new LiteralTag { Content = new [] { new StringSlice(" ") } },
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_LambdaTags_WithContext()
        {
            const string result = "1 Bar";

            var context = new Context(
                new
                {
                    lambda = new Func<dynamic, string, object>((dyn, str) => $"1 {dyn.bar}"),
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "lambda",
                    Children = new List<MustacheTag>
                    {
                    }
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }

        [Fact]
        public void It_Can_Render_LambdaTags_UsingOriginalTemplate()
        {
            const string result = "<b>a b c Bar d e f</b>";

            var context = new Context(
                new
                {
                    lambda = new Func<string, object>(str => $"<b>{str}</b>"),
                    bar = "Bar"
                },
                new RendererSettingsBuilder().BuildSettings(),
                RenderSettings.GetDefaultRenderSettings());

            var stringRenderer = new StringRender(StreamWriter);
            var sectionTokenRenderer = new SectionTokenRenderer();

            sectionTokenRenderer.Write(
                stringRenderer,
                new SectionTag
                {
                    SectionName = "lambda",
                    SectionContent = new StringSlice("a b c {{bar}} d e f")
                },
                context);

            StreamWriter.Flush();
            MemStream.Position = 0;
            var sr = new StreamReader(MemStream);
            var myStr = sr.ReadToEnd();
            Assert.Equal(result, myStr);
        }
    }
}
