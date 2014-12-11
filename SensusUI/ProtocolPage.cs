﻿using SensusService;
using SensusService.DataStores;
using SensusUI.UiProperties;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SensusUI
{
    public class ProtocolPage : ContentPage
    {
        public static event EventHandler<ProtocolDataStoreEventArgs> EditDataStoreTapped;
        public static event EventHandler<ProtocolDataStoreEventArgs> CreateDataStoreTapped;
        public static event EventHandler<ItemTappedEventArgs> ProbeTapped;

        private class DataStoreValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return parameter + " store:  " + (value == null ? "None" : (value as DataStore).Name);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class ProbeDetailValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                Datum mostRecent = value as Datum;
                return mostRecent == null ? "----------" : mostRecent.DisplayDetail + Environment.NewLine + mostRecent.Timestamp;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public ProtocolPage(Protocol protocol)
        {
            BindingContext = protocol;

            SetBinding(TitleProperty, new Binding("Name"));

            List<View> views = new List<View>();

            views.AddRange(UiProperty.GetPropertyStacks(protocol));

            #region probes
            ListView probesList = new ListView();
            probesList.ItemTemplate = new DataTemplate(typeof(TextCell));
            probesList.ItemTemplate.SetBinding(TextCell.TextProperty, "DisplayName");
            probesList.ItemTemplate.SetBinding(TextCell.DetailProperty, new Binding("MostRecentlyStoredDatum", converter: new ProbeDetailValueConverter()));
            probesList.ItemsSource = protocol.Probes;
            probesList.ItemTapped += (o, e) =>
                {
                    probesList.SelectedItem = null;
                    ProbeTapped(o, e);
                };

            views.Add(probesList);
            #endregion

            #region data stores
            Button editLocalDataStoreButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Font = Font.SystemFontOfSize(20),
                BindingContext = protocol
            };

            editLocalDataStoreButton.SetBinding(Button.TextProperty, new Binding("LocalDataStore", converter: new DataStoreValueConverter(), converterParameter: "Local"));
            editLocalDataStoreButton.Clicked += (o, e) =>
                {
                    DataStore copy = null;
                    if (protocol.LocalDataStore != null)
                        copy = protocol.LocalDataStore.Copy();

                    EditDataStoreTapped(o, new ProtocolDataStoreEventArgs { Protocol = protocol, DataStore = copy, Local = true });
                };

            Button createLocalDataStoreButton = new Button
            {
                Text = "+",
                HorizontalOptions = LayoutOptions.End,
                Font = Font.SystemFontOfSize(20)
            };

            createLocalDataStoreButton.Clicked += (o, e) =>
                {
                    CreateDataStoreTapped(o, new ProtocolDataStoreEventArgs { Protocol = protocol, Local = true });
                };

            StackLayout localDataStoreStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { editLocalDataStoreButton, createLocalDataStoreButton }
            };

            views.Add(localDataStoreStack);

            Button editRemoteDataStoreButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Font = Font.SystemFontOfSize(20),
                BindingContext = protocol
            };

            editRemoteDataStoreButton.SetBinding(Button.TextProperty, new Binding("RemoteDataStore", converter: new DataStoreValueConverter(), converterParameter: "Remote"));
            editRemoteDataStoreButton.Clicked += (o, e) =>
                {
                    DataStore copy = null;
                    if (protocol.RemoteDataStore != null)
                        copy = protocol.RemoteDataStore.Copy();

                    EditDataStoreTapped(o, new ProtocolDataStoreEventArgs { Protocol = protocol, DataStore = copy, Local = false });
                };

            Button createRemoteDataStoreButton = new Button
            {
                Text = "+",
                HorizontalOptions = LayoutOptions.End,
                Font = Font.SystemFontOfSize(20)
            };

            createRemoteDataStoreButton.Clicked += (o, e) =>
                {
                    CreateDataStoreTapped(o, new ProtocolDataStoreEventArgs { Protocol = protocol, Local = false });
                };

            StackLayout remoteDataStoreStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { editRemoteDataStoreButton, createRemoteDataStoreButton }
            };

            views.Add(remoteDataStoreStack);
            #endregion

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
            };

            foreach (View view in views)
                (Content as StackLayout).Children.Add(view);

            ToolbarItems.Add(new ToolbarItem("Share", null, () =>
                {
                    string path = null;
                    try
                    {
                        path = UiBoundSensusServiceHelper.Get().GetTempPath(".sensus");
                        protocol.Save(path);
                    }
                    catch (Exception ex)
                    {
                        UiBoundSensusServiceHelper.Get().Logger.Log("Failed to save protocol to file for sharing:  " + ex.Message, LoggingLevel.Normal);
                        path = null;
                    }

                    if (path != null)
                        UiBoundSensusServiceHelper.Get().ShareFile(path, "Sensus Protocol:  " + protocol.Name);
                }));
        }
    }
}
