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
        $scope.bomb = {
            planting: false,
            planted: false,
            ticking: false,
            exploded: false,
            defused: false,
        };

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
        //var stepExec = function stepExec(thisTick, nextTick) {
        //    if (thisTick) {
        //        thisTick();
        //    }
        //    if (nextTick) {
        //        $timeout(function () { nextTick() }, 0);
        //    }
        //}
        var playerStepExec = function playerStepExec(userid, thisTick, nextTick) {
            for (var i = 0; i < $scope.players.length; i++) {
                var p = $scope.players[i];
                if (p.Id == userid) {

                    if (thisTick) {
                        thisTick(p);
                    }
                    if (nextTick) {
                        $timeout(function () { nextTick(p) }, 0);
                    }

                    break;
                }
            }
        }

        $scope.hubRaiseEvent = function hubRaiseEvent(serverId, e) {
            switch (e.name) {
                case GameEvent.weapon_fire:
                    playerStepExec(e.userid, function (p) { p.weapon_fire = true; }, function (p) { p.weapon_fire = false; })
                    return;
                case GameEvent.enter_buyzone:
                    playerStepExec(e.userid, function (p) {
                        p.buyzone_in = true;
                        p.buyzone_canbuy = e.canbuy;
                    });
                    return;
                case GameEvent.exit_buyzone:
                    playerStepExec(e.userid, function (p) {
                        p.buyzone_in = false;
                        p.buyzone_canbuy = e.canbuy;
                    });
                    return;
                case GameEvent.bomb_beginplant:
                    $scope.bomb.planting = true;
                    break;
                case GameEvent.bomb_abortplant:
                    $scope.bomb.planting = false;
                    break;
                case GameEvent.bomb_planted:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = true;
                    break;
                case GameEvent.bomb_defused:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = false;
                    break;
                case GameEvent.bomb_exploded:
                    $scope.bomb.planting = false;
                    $scope.bomb.planted = true;
                    break;
                case GameEvent.bomb_beep:
                    $scope.bomb.ticking = true;
                    $timeout(function () { $scope.bomb.ticking = false; }, 0);
                    break;
                    //case GameEvent.entity_killed:
                    //case GameEvent.player_death:
                    //case GameEvent.bomb_begindefuse:
                    //case GameEvent.bomb_abortdefuse:

                case GameEvent.enter_bombzone:
                    playerStepExec(e.userid, function (p) {
                        p.bombzone_in = true;
                        p.bombzone_hasbomb = e.hasbomb;
                    });
                    return;
                case GameEvent.exit_bombzone:
                    playerStepExec(e.userid, function (p) {
                        p.bombzone_in = false;
                        p.bombzone_hasbomb = e.hasbomb;
                    });
                    return;
                    //case GameEvent.bomb_dropped:
                    //case GameEvent.bomb_pickup:
                    //    break;

                case GameEvent.player_hurt:
                    playerStepExec(e.userid, function (p) {
                        p.Health = e.health;
                        p.Armor = e.armor;
                    })
                    return;

                case GameEvent.player_jump:
                case GameEvent.weapon_reload:
                case GameEvent.item_pickup:
                case GameEvent.player_avenged_teammate:
                case GameEvent.item_equip:
                    //Ignoring
                    return;
            }
            $scope.lastEvents.splice(0, 0, e);
            while ($scope.lastEvents.length > 50) {
                $scope.lastEvents.splice(49, 1);
            }
        };

        connect();
    }])