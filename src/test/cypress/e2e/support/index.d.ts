declare namespace Cypress {
  interface Chainable {
    /**
     * Custom command to create an org using gitea api
     * @example cy.createorg(ttd, token)
     */
    createorg(orgname: string, token: string): Chainable<Element>;

    /**
     * Custom command to delete an org using gitea api
     * @example cy.deleteorg(ttd, token)
     */
    deleteorg(orgname: string, token: string): Chainable<Element>;

    /**
     * Custom command to delete all apps of an org using gitea api
     * @example cy.deleteallapps(ttd, token)
     */
    deleteallapps(orgname: string, token: string): Chainable<Element>;

    /**
     * Custom command to make an user owner of an org using gitea api
     * @example cy.makeuserowner(ttd, testuser, token)
     */
    makeuserowner(orgname: string, username: string, token: string): Chainable<Element>;

    /**
     * Custom command to delete an user using gitea api
     * @example cy.deleteuser(testuser, token)
     */
    deleteuser(username: string, token: string): Chainable<Element>;

    /**
     * Custom command to login to studio with username and pwd
     * @example cy.studiologin(testuser, userpwd)
     */
    studiologin(username: string, userpwd: string): Chainable<Element>;

    /**
     * Custom command to create an app from studio dashboard
     * @example cy.createapp(ttd, testapp)
     */
    createapp(orgname: string, appname: string): Chainable<Element>;

    /**
     * Select and delete all the added ui components in an app's ui editor
     * @example cy.deletecomponents()
     */
    deletecomponents(): Chainable<Element>;

    /**
     * Custom command to get an altinnstudioruntime token for an app owner
     * @example cy.getTokenForOrg(ttd)
     */
    getTokenForOrg(orgname: string): Chainable<Element>;

    /**
     * Custom command to upload an attachment to an app instance
     * @example cy.uploadAttachment(ttd, app, "512345", "fc56bb33-eb24-4583-967d-7cf2c2d5fa53", "attachment", "jwttoken")
     */
    uploadAttachment(
      orgname: string,
      appname: string,
      partyId: string,
      instanceId: string,
      attachmentId: string,
      token: string,
    ): Chainable<Element>;

    /**
     * Custom command to navigate to change name layout in task_2 in app: frontend-test
     * @example cy.navigateToChangeName()
     */
    navigateToChangeName(): Chainable<Element>;

    /**
     * Custom command to used in beforeeach that preserves cookies between tests
     * @example cy.preserveCookies()
     */
    preserveCookies(): Chainable<Element>;

    /**
     * Custom command to complete the change name form with supplied names and move to next page
     * @example cy.completeChangeNameForm('abc', 'xyz')
     */
    completeChangeNameForm(firstName: string, lastName: string): Chainable<Element>;

    /**
     * Start an app instance based on the environment selected
     * @example cy.startAppInstance('appName')
     */
    startAppInstance(appName: string): Chainable<Element>;

    /**
     * Navigate to the task3 of app ttd/frontend-test
     * @example cy.navigateToTask3()
     */
    navigateToTask3(): Chainable<Element>;

    /**
     * navigate to task 3 and complete task 3 form
     * @example cy.compelteTask3Form()
     */
    compelteTask3Form(): Chainable<Element>;

    /**
     * Add an item to group component with an item in nested group
     * @example cy.addItemToGroup(1, 2, 'automation')
     */
    addItemToGroup(oldValue: Number, newValue: Number, comment: string): Chainable<Element>;

    /**
     * Test for WCAG violations of impact critical, serious, moderate
     * @example cy.testWcag()
     */
    testWcag(): Chainable<Element>;

    /**
     * Reset local repo of an app
     * @example cy.deleteLocalChanges('test/test-app')
     */
    deleteLocalChanges(appId: String): Chainable<Element>;

    /**
     * Get body of ifram from the DOM
     * @example cy.getIframeBody()
     */
    getIframeBody(): Chainable<Element>;

    /**
     * check visibility of an element whose parent is found hidden by cypress
     */
    isVisible(): Chainable<Element>;

    /**
     * Custom command to search and open an app
     * @example cy.searchAndOpenApp('ttd/app')
     */
    searchAndOpenApp(appId: string): Chainable<Element>;

    /**
     * Custom command to create a repo for an user
     * @example cy.createrepository(user, app, token)
     */
    createrepository(username: string, appName: string, token: string): Chainable<Element>;

    /**
     * Custom command to get a repo and return response
     * @example cy.getrepo('ttd/app', token)
     */
    getrepo(appId: string, token: string): Chainable<Element>;

    /**
     * Instantiate statefull instance from ttd/stateless-app
     * @example cy.startStateFullFromStateless()
     */
     startStateFullFromStateless(): Chainable<Element>;
  }
}
