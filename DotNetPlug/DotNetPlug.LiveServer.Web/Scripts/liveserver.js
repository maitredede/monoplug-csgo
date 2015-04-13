"use strict";

angular.module("liveserver", ["SignalR", "ui.router", "ui.bootstrap"])
//.factory("WebUIHub", ["$rootScope", "Hub", "$timeout",
//    function ($rootScope, Hub, $timeout) {

//        //declaring the hub connection
//        var hub = new Hub('webUI', {

//            ////client side methods
//            listeners: {
//                "RaiseEvent": function (serverId, e) {
//                    debugger;
//                }
//                //    'lockEmployee': function (id) {
//                //        var employee = find(id);
//                //        employee.Locked = true;
//                //        $rootScope.$apply();
//                //    },
//                //    'unlockEmployee': function (id) {
//                //        var employee = find(id);
//                //        employee.Locked = false;
//                //        $rootScope.$apply();
//                //    }
//            },

//            //server side methods
//            methods: ['hello'],

//            ////query params sent on initial connection
//            //queryParams: {
//            //    'token': 'exampletoken'
//            //},

//            //handle connection error
//            errorHandler: function (error) {
//                console.error(error);
//            },

//            //specify a non default root
//            //rootPath: '/api

//            hubDisconnected: function () {
//                if (hub.connection.lastError) {
//                    hub.connection.start()
//                        .done(function () {
//                            if (hub.connection.state === 0) {
//                                $timeout(function () {
//                                    //your code here 
//                                }, 2000);
//                            } else {
//                                //your code here
//                            }
//                        })
//                        .fail(function (reason) {
//                            console.log(reason);
//                        });
//                }
//            }
//        });

//        //var edit = function (employee) {
//        //    hub.lock(employee.Id); //Calling a server method
//        //};
//        //var done = function (employee) {
//        //    hub.unlock(employee.Id); //Calling a server method
//        //}

//        //return {
//        //    editEmployee: edit,
//        //    doneWithEmployee: done
//        //};
//        return {
//            hello: function (serverId) {
//                var helloResult = hub.hello(serverId);
//                return helloResult;
//            }
//        }
//    }])
.controller("demoController", ["$scope", "Hub", "$timeout", "GameEvent",
    function ($scope, Hub, $timeout, GameEvent) {
        $scope.error = "";
        $scope.lastEvents = [];
        $scope.players = [];

        $scope.serverId = "Demo";

        var hub = null;
        $scope.hubConnectDone = function hubConnectDone() {
            $scope.error = "connected : (status=" + hub.connection.state + ")";
            hub.hello($scope.serverId).done(function () {
                $scope.$apply(function () {
                    $scope.error = "listening...";
                })
            });
        };

        $scope.hubConnectFail = function hubConnectFail(reason) {
            console.log(reason);
            $scope.error = reason;
        };

        var connect = function () {
            hub.connection.start()
                .done(function (e) {
                    $scope.$apply(function () {
                        $scope.hubConnectDone();
                    });
                })
                .fail(function (reason) {
                    $scope.$apply(function () {
                        $scope.hubConnectFail(reason);
                    });
                });
        };

        hub = new Hub("webUIHub", {
            listeners: {
                RaiseEvent: function (serverId, e) {
                    $scope.$apply(function () {
                        $scope.hubRaiseEvent(serverId, e);
                    });
                },
                SetPlayers: function (serverId, players) {
                    $scope.$apply(function () {
                        $scope.hubSetPlayers(serverId, players);
                    });
                }
            },
            methods: ["hello"],
            errorHandler: function (error) {
                console.error(error);
                $scope.error = error;
            },
            hubDisconnected: function () {
                $scope.$apply(function () {
                    $scope.error = "connecting...";
                });
                connect();
            }
        });

        $scope.hubSetPlayers = function hubSetPlayers(serverId, players) {
            $scope.players = players;
        }

        $scope.hubRaiseEvent = function hubRaiseEvent(serverId, e) {
            switch (e.name) {
                case GameEvent.player_hurt:
                case GameEvent.weapon_fire:
                    return;
                    //break;
                case GameEvent.round_start:
                    break;
                    break;
            }
            $scope.lastEvents.splice(0, 0, e);
            while ($scope.lastEvents.length > 50) {
                $scope.lastEvents.splice(49, 1);
            }
        };

        connect();
    }])