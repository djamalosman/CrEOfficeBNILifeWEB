Handlebars.registerHelper('formatDate', function (date) {
    var mmnt = moment(date);
    return mmnt.format('DD-MM-YYYY');
});