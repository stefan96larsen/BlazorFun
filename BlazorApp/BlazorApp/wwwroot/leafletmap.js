let dotnetHelper;
let drawnItems;

export function initialize_map(dotnetObject) {
    dotnetHelper = dotnetObject;

    const map = L.map('map').setView([55.4038, 10.4024], 14);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {maxZoom: 19})
        .addTo(map);

    drawnItems = new L.FeatureGroup().addTo(map);

    const drawControl = new L.Control.Draw({
        edit: {
            featureGroup: drawnItems
        },
        draw: {
            polygon: true,
            polyline: false,
            circle: false,
            rectangle: false,
            marker: false,
            circlemarker: false
        }
    });
    map.addControl(drawControl);

    map.on(L.Draw.Event.CREATED, e => {
        const layer = e.layer;
        drawnItems.addLayer(layer);
        const geojson = layer.toGeoJSON();
        dotnetHelper.invokeMethodAsync('ShapeCreated', JSON.stringify(geojson));
    });

    map.on(L.Draw.Event.EDITED, e => {
        e.layers.eachLayer(layer => {
            const geojson = layer.toGeoJSON();
            dotnetHelper.invokeMethodAsync('ShapeEdited', JSON.stringify(geojson));
        });
    });

    map.on(L.Draw.Event.DELETED, e => {
        e.layers.eachLayer(layer => {
            const geojson = layer.toGeoJSON();
            dotnetHelper.invokeMethodAsync('ShapeDeleted', JSON.stringify(geojson));
        });
    });
}