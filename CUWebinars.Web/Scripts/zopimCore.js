
function SendUserToCertHelp(order) {
    $zopim.livechat.sendVisitorPath({
        url: 'http://www.bankwebinars.com/CertHelp',
        title: 'About Your Certificate'
    });
}
function SendUserToPasswordHelp(user) {
    $zopim(function () {
        $zopim.livechat.sendVisitorPath({
            url: 'http://www.bankwebinars.com/UserHelp',
            title: 'About Your Account'
        });
    });
}

function SendUserToOrderHelp(order) {
    $zopim.livechat.sendVisitorPath({
        url: 'http://www.bankwebinars.com/OrderHelp',
        title: 'About Your Registration'
    });
}

$zopim(function () {

    $zopim.livechat.setGreetings({
        'online': 'Here to help',
        'offline': 'Leave a message'
    });
    $zopim.livechat.button.show();
    $zopim.livechat.bubble.hide();
    var bubble = 'online';

    window.$zopim.livechat.bubble.setTitle('Help & Feedback');
    window.$zopim.livechat.bubble.setText('Click to Chat');
});
