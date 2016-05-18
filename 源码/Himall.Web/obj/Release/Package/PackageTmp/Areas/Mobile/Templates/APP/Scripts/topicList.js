$(function () {
    LoadTopic();
})

function LoadTopic()
{
    $.post('/' + areaName + '/Comment/AddComment')
}