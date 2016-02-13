describe('E2E: main page', function() {

    var ptor;
    beforeEach(function () {
        "use strict";

        var url = 'http://localhost/#/sitter/';
        browser.get( url );
    });

    it('should load sitter page', function() {
        var ele = by.id('labName');
        expect(browser.isElementPresent(ele)).toBe(true);

        ele = by.id('assayType');
        expect(browser.isElementPresent(ele)).toBe(true);
    });

    it('should angular bind vm', function() {
        expect(true).toBe(true);
        /*
         var ele = by.id('nr1');
         expect(ptor.isElementPresent(ele)).toBe(true);
         */

        element(by.id('labName')).clear();
        var labName = 'Protractor test lab';
        element(by.id('labName')).sendKeys(labName);


        // This wouldn't work https://github.com/angular/protractor/blob/master/docs/faq.md#the-result-of-gettext-from-an-input-element-is-always-empty
        //expect(element(by.model('vm.acquisition.labName')).getText()).toEqual('');
        // Instead use this
        expect(element(by.model('vm.sitter.name')).getAttribute('value')).toEqual(labName);
    });


});