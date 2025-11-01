(function(){
    if (!window.Ext || !Ext.application) return;
    if (!window.App) window.App = {};
    Ext.application({
        name: 'App',
        launch: function(){
            // Instantiate bridge singletons to ensure scanning current DOM
            try { Ext.create('App.grid.DataGrid'); } catch(e) {}
            try { Ext.create('App.filter.GridFilter'); } catch(e) {}
        }
    });
})();





