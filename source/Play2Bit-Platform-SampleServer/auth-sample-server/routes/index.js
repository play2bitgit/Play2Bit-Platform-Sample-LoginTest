const http = require('http');
const https = require('https');
const express = require('express');
const url = require('url');
const router = express.Router();

const clientID = "2OezmFJLKiE3nsxa";
const clientSecret = "h7X6oVpB64i0vIDrAu7UYbf5zlEzzvDk";
const clientBase64 = Buffer.from(clientID + ':' + clientSecret).toString('base64');

const authHost = 'auth.play2bit.com';
const authPort = 443;
const apiHost = 'api.play2bit.com';
const apiPort = 443;

const redirectUri = encodeURIComponent('http://dev-auth-sample-web.play2bit.com/oauth/callback');

router.get('/', function (req, res, next) {
    res.render('index', {
        title: 'Play2Bit Oauth Example Server'
    });
});

router.get('/oauth/callback', function (req, res, next) {
    return res.json(req.query);
});

router.post('/app/token/account', function (req, res, next) {
    const userAccessToken = req.body.access_token;

    requestAccount(userAccessToken, account => {
        return res.json(account);
    });
});

router.post('/oauth/token', function (req, res, next) {
    const data = `grant_type=authorization_code&code=${req.body.code}&redirect_uri=${redirectUri}`;

    requestToken(data, token => {
        return res.json(token);
    });
});

function requestToken(data, cb) {
    const req = https.request({
        host: authHost,
        port: authPort,
        method: 'post',
        path: '/v1/oauth/token',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Authorization': 'Basic ' + clientBase64
        }
    }, res => {
        let body = '';
        res.on('data', chunk => body += chunk);
        res.on('end', () => cb(JSON.parse(body)));
    });
    req.on('error', e => console.error(`problem with request: ${e.message}`));
    req.write(data);
    req.end();
}

function requestAccount(userAccessToken, cb) {
    const req = https.request({
        host: apiHost,
        port: apiPort,
        method: 'get',
        path: '/v1/system/maintenance',
        headers: {
            'Content-Type' : 'application/json',
            'Authorization' : 'Bearer ' + userAccessToken
        }
    }, res => {
        let body = '';
        res.on('data', chunk => body += chunk);
        res.on('end', () => cb(JSON.parse(body)));
    });
    req.on('error', e => console.error(`problem with request: ${e.message}`));
    req.end();
}

module.exports = router;
