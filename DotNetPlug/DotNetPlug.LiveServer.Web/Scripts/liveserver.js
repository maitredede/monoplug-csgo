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
.controller("demoController", ["$scope", "Hub", "$timeout",
    function ($scope, Hub, $timeout) {
        $scope.error = "";
        $scope.lastEvents = [];
        $scope.players = [];

        $scope.serverId = "Demo";

        var hub = null;
        var connect = function () {
            hub.connection.start()
                        .done(function (e) {
                            //if (hub.connection.state === 0) {
                            //    $scope.$apply(function () {
                            //        $scope.error = "connected";
                            //    });
                            //} else {
                            //    $scope.$apply(function () {
                            //        $scope.error = "connection error";
                            //    });
                            //}
                            $scope.$apply(function () {
                                $scope.error = "connected : (status=" + hub.connection.state + ")";
                                var hello = hub.hello($scope.serverId);
                                hello.done(function () {
                                    $scope.$apply(function () {
                                        $scope.error = "listening...";
                                    })
                                });
                            });
                        })
                        .fail(function (reason) {
                            console.log(reason);
                            $scope.$apply(function () {
                                $scope.error = reason;
                            });
                        });
        };
        hub = new Hub("webUIHub", {
            listeners: {
                RaiseEvent: function (serverId, e) {
                    $scope.$apply(function () {
                        $scope.lastEvents.splice(0, 0, e);
                        while ($scope.lastEvents.length > 50) {
                            $scope.lastEvents.splice(49, 1);
                        }
                    });
                },
                SetPlayers: function (players) {
                    $scope.$apply(function () {
                        $scope.players = players;
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
            //hubDisconnected: function () {
            //    $scope.$apply(function () {
            //        $scope.error = "connecting...";
            //    });
            //    //if (hub.connection.lastError) {
            //    hub.connection.start()
            //        .done(function () {
            //            if (hub.connection.state === 0) {
            //                $scope.$apply(function () {
            //                    $scope.error = "connected";
            //                });
            //            } else {
            //                $scope.$apply(function () {
            //                    $scope.error = "connection error";
            //                });
            //            }
            //        })
            //        .fail(function (reason) {
            //            console.log(reason);
            //            $scope.$apply(function () {
            //                $scope.error = reason;
            //            });
            //        });
            //    //} else {
            //    //    $scope.$apply(function () {
            //    //        $scope.error = "connecting...";
            //    //    });
            //    //}
            //}

        });
        connect();
    }])