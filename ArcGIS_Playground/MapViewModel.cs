using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;

namespace ArcGIS_Playground
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public Esri.ArcGISRuntime.UI.Controls.MapView MapView;
        private LocatorTask _geocoder;

        private IReadOnlyList<SuggestResult> _suggestions;
        public IReadOnlyList<SuggestResult> AddressSuggestions
        {
            get { return _suggestions; }
            set
            {
                _suggestions = value;
                OnPropertyChanged();
            }
        }

        public MapViewModel()
        {
            CreateNewMap();
            _geocoder = new LocatorTask(new Uri("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer"));
            _geocoder.LoadAsync();
            _map.InitialViewpoint = new Viewpoint(34.05293, -118.24368, 600000);
            
        }

        private Map _map = new Map(Basemap.CreateImagery());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void CreateNewMap()
        {
            Map newMap = new Map(Basemap.CreateImageryWithLabels());
            FeatureLayer trailHeadsLayer = new FeatureLayer(new Uri("https://services1.arcgis.com/vxtcdWjsjPHYpR1J/arcgis/rest/services/Onderwijsinstellingen/FeatureServer/0") ); 
            //new FeatureLayer(new Uri("https://services3.arcgis.com/GVgbJbqm8hXASVYi/arcgis/rest/services/Trailheads/FeatureServer/0"));
            await trailHeadsLayer.LoadAsync();
            trailHeadsLayer.IsPopupEnabled = true;
            newMap.OperationalLayers.Add(trailHeadsLayer);
            
            newMap.InitialViewpoint = new Viewpoint(trailHeadsLayer.FullExtent);
            Map = newMap;
        }

        public async void GetAddressSuggestions(string searchText)
        {
            if (_geocoder.LocatorInfo.SupportsSuggestions)
            {
                Geometry currentExtent = MapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                SuggestParameters suggestParams = new SuggestParameters { MaxResults = 10, SearchArea = currentExtent };
                IReadOnlyList<SuggestResult> suggestions = await _geocoder.SuggestAsync(searchText, suggestParams);
                AddressSuggestions = suggestions;
            }
        }
    }
}
