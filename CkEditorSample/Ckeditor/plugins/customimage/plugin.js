CKEDITOR.plugins.add('customimage',
{
    init: function (editor) {
        var pluginName = 'customimage';
        editor.ui.addButton('customimage',
            {
                label: 'My New Plugin',
                command: 'OpenWindow',
                icon: CKEDITOR.plugins.getPath('customimage') + 'ImageIcon.png'
            });
        var cmd = editor.addCommand('OpenWindow', { exec: showMyDialog });
    }
});

