//var assert = require('assert')
try {
    //var chai = require('chai');
    var expect = chai.expect,
        should = chai.should;

    //require('angular');
}
catch (ex) {
    console.log(ex);
};

var $controllerConstructor;
var scope;

describe('Job controller', function () {
    beforeEach(module('app'));

    beforeEach(inject(function($controller, $rootScope){
        $controllerConstructor = $controller;
        scope = $rootScope.$new();
    }));


    it('should dummy pass', function () {
        expect(1).to.equal(1);
    });



});