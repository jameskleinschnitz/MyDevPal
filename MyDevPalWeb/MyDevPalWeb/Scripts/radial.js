
angular.module('myDevPal', [])
  .controller('mdpCtrl', ['$http', '$timeout', function ($http, $timeout) {
      var mdp = this;

      mdp.score = {};
      mdp.icons = {};
      mdp.actions = [];
      mdp.counter = 0;

      mdp.icons.smile = '\uF118';
      mdp.icons.frown = '\uF119';
      mdp.icons.coffee = '\uF0F4';
      mdp.icons.target = '\uF192';
      mdp.icons.batteryFull = '\uF240';
      mdp.icons.batteryEmpty = '\uF244';
      mdp.icons.batteryHalf = '\uF244';
      mdp.icons.batteryQuarter = '\uF244';
      mdp.icons.batteryThreeQuarter = '\uF244';
      mdp.icons.bed = '\uF236';

      //init chart
      var mainChart = new RadialProgressChart('.single', {
          diameter: 100,
          series: [{
              labelStart: mdp.icons.smile, //smile
              value: 50
          }, {
              labelStart: mdp.icons.batteryFull, 
              value: 100
          }, {
              labelStart: mdp.icons.target, //target
              value: 100
          }]
      });

      //init messages
      $http.get("https://devstg6.cireson.com/api/DevStatAlteration?$orderby=Id%20desc&$top=5").then(function (resp) {
          //console.log('Success', resp);
          // For JSON responses, resp.data contains the result
          angular.forEach(resp.data.value, function (value, key) {

              if (value.MoodDelta != 0 ||
                  value.VitalityDelta != 0 ||
                  value.FocusDelta != 0
                  ) {
                  var actionMsg = value;
                  mdp.actions.splice(0, 0, actionMsg);


                  //update radial
                  mdp.updateRadial(value.Mood, value.Vitality, value.Focus);
              }
              mdp.lastId = value.Id;

          });
         

      }, function (err) {
          console.error('ERR', err);
          // err.status will contain the status code

      });


      mdp.refresh = function () {

     

          //update messages
          $http.get("https://devstg6.cireson.com/api/DevStatAlteration?$filter=(Id%20gt%20" + mdp.lastId + ")&$orderby=Id%20asc").then(function (resp) {
              //console.log('Success', resp);
              // For JSON responses, resp.data contains the result
              angular.forEach(resp.data.value, function (value, key) {

                  if (value.MoodDelta != 0 ||
                  value.VitalityDelta != 0 ||
                  value.FocusDelta != 0
                  ) {
                      var actionMsg = value;
                      var remove = 0;
                      if (mdp.actions.length > 5)
                          remove = 1;

                      //don't make the queue to big
                      mdp.actions.splice(0, 0, actionMsg);
                      mdp.actions.splice(6, remove);
 
                      //update radial
                      mdp.updateRadial(value.Mood, value.Vitality, value.Focus);
                  }

                  mdp.lastId = value.Id;
              });

              

          }, function (err) {
              console.error('ERR', err);
              // err.status will contain the status code

          });

          $timeout(mdp.refresh, 1000);
          mdp.counter++;
      };

      mdp.refresh();

      mdp.updateRadial = function (CurrentMood, CurrentVitality,CurrentFocus) {
          if (mdp.score.CurrentMood != CurrentMood ||
                mdp.score.CurrentVitality != CurrentVitality ||
                mdp.score.CurrentFocus != CurrentFocus) {

              mdp.score.CurrentMood = CurrentMood;
              mdp.score.CurrentVitality = CurrentVitality;
              mdp.score.CurrentFocus = CurrentFocus;

              //try to update icons
              mdp.icons.CurrentMood = mdp.icons.smile;
              mdp.icons.CurrentVitality = mdp.icons.batteryFull;
              mdp.icons.CurrentFocus = mdp.icons.target;

              //show a frown
              if (mdp.score.CurrentMood < 45) {
                  mdp.icons.CurrentMood = mdp.icons.frown;
              }

              var series = [{
                  label: mdp.icons.CurrentMood,
                  value: mdp.score.CurrentMood
              }, {
                  label: mdp.icons.CurrentVitality,
                  value: mdp.score.CurrentVitality
              }, {
                  label: mdp.icons.CurrentFocus,
                  value: mdp.score.CurrentFocus
              }];

              mainChart.update(series);
          }
      }
     
  }]);
