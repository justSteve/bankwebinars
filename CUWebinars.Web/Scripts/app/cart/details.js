var pageObject,
    stateManager;

$(function() {

    pageObject = new OrderRegistration.PageObject();

    stateManager = new OrderRegistration.StateManager(pageObject);

    stateManager.SetCartState();
});