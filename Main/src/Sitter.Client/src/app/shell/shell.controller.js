angular.module('appControllers').controller('ShellController', [
	'$scope',
	'$route',
	'$rootScope',
	'$location',
	'userFactory',
	'globalsFactory',
	function ($scope, $route, $rootScope, $location, userFactory, globalsFactory) {
		// Function to collapse/expand sidebar.
		$scope.collapseSidebarOnClick = function () {
			$scope.SidebarHelper.sidebarCollapsed = !$scope.SidebarHelper.sidebarCollapsed;
		};
		// Function to understand if certain link is active.
		$scope.isActive = function (viewLocation) {
			var active = (viewLocation === "#" + $location.path());
			return active;
		};
		// Init scope.
		var initScope = function () {
			$scope.SidebarHelper = {
				sidebarCollapsed: false
			};
			// Menu links.
			$scope.SidebarHelper.menuItems = [
				{
					text: "Post A Job",
					id: 'parent-post-job',
					url: '#/parent/post-job',
					type: 'button',
					role: "parent"
				}, {
					text: "My Jobs",
					id: 'parent-my-jobs',
					url: '#/parent/myjobs',
					type: 'menu-item',
					role: "parent"
				}, {
					text: "My Sitters",
					id: 'parent-my-sitters',
					url: '#/parent/mysitters',
					type: 'menu-item',
					role: "parent"
				}, {
					text: "Parent Profile",
					id: 'parent-profile',
					url: '#/parent/profile',
					type: 'menu-item',
					role: "parent"
				}, {
					text: "My Jobs",
					id: 'sitter-my-jobs',
					url: '#/sitter/myjobs',
					type: 'menu-item',
					role: "sitter"
				}, {
					text: "Sitter Profile",
					id: 'sitter-profile',
					url: '#/sitter/profile',
					type: 'menu-item',
					role: "sitter"
				}, {
					text: "My Clients",
					id: 'sitter-network',
					url: '#/sitter/myclients',
					type: 'menu-item',
					role: "sitter"
				}, {
					text: "SMS Simulator",
					id: 'sms-simulator',
					url: '#/admin/sms-simulator',
					type: 'menu-item',
					role: "admin"
				}, {// TODO: really! we need to make role as an array.
					text: "Log Out",
					id: 'login-logout',
					url: '#/user/login?logout=true',
					type: 'menu-item',
					role: "sitter"
				}, {
					text: "Log Out",
					id: 'login-logout',
					url: '#/user/login?logout=true',
					type: 'menu-item',
					role: "parent"
				}, {
					text: "Log Out",
					id: 'login-logout',
					url: '#/user/login?logout=true',
					type: 'menu-item',
					role: "admin"
				}];
			// Watch for changes in rootScope's globalData.
			globalsFactory.watchGlobalData($scope);
		};

		initScope();
	}
]);