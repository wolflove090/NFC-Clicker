mergeInto( LibraryManager.library, {
    SetFsSyncfsToUnloadEvent: function( prefsSaveFunctionPtr ) {
        const beforeunloadListener = (event) => {
            dynCall( "v", prefsSaveFunctionPtr, [] );
            FS.syncfs( false, function( err ){} );
            window.removeEventListener( 'beforeunload', beforeunloadListener );
        };
        window.addEventListener( 'beforeunload', beforeunloadListener );
    },
});
