// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class EllipticalPanelPage : Page, IXamlRenderListener
    {
        public EllipticalPanelPage()
        {
            this.InitializeComponent();
        }

        public void OnXamlRendered(FrameworkElement element)
        {
        }
    }
}