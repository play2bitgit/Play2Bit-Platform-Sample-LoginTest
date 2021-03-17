# Login-Test App Guide

Play2Bit Platform Login 연동을 테스트할 수 있는 Login-Test App 을 제공합니다.

본 문서는 **Login-Test App** 을 동작시키기 위한 방법을 설명합니다.

#

## 환경정보

1. 구성은 아래와 같이 두가지 모듈로 구성됩니다.

   A. 클라이언트 앱 : 웹뷰를 이용한 로그인 처리와 지갑정보를 표시

   B. API 서버 : Play2Bit Platform API 와 커뮤니케이션 담당

2. **클라이언트 앱**은 아래 환경으로 테스트 되었습니다.

   A. 유니티 v2018.4.6f1 LTS

   B. 안드로이드 8.0 이상 (오레오)

   C. WebView : GreeWebView (https://github.com/gree/unity-webview)

3. **API 서버**는 아래 환경으로 테스트 되었습니다.

   A. Node.js v12.14.0 LTS

```
  주의)
  Access Token 요청이나 확인에 필요한 **Client Secret** 은 인증을 위한 매우 중요한 키 값입니다.
  따라서, 외부에 절대 노출되지 않도록 주의해야 하며, 반드시 개발사 내부 서버를 통해 auth 서버로 호출하여야 합니다.
  클라이언트 앱 소스 코드 내에 **Client Secret** 을 저장하는 사례가 있는데 이 경우 외부에 노출될 확율이 높아 보안이 매우 취약해 집니다.
  샘플 API 서버는 위와 같은 사유로 당사의 서버를 이용하도록 샘플 앱 소스 코드에 적용되어 있으므로 서버를 실행하지 않아도 확인이 가능합니다.

  Live 서비스를 위해 인증에 필요한 서버는 필수적으로 개발하여야 하며, 이 떼 샘플 API 서버를 참고하시면 됩니다.
```

#

## 구현된 기능스펙

1. 인증 코드 요청 페이지 접근

- 참고 : https://developer-guide.play2bit.com/api/auth-api/#1

2. 사용자 Access Token 요청 및 발급

- 참고 : https://developer-guide.play2bit.com/api/auth-api/#2-access-token

#

## Login-Test App 설치 및 실행

1. 유니티 설치

```
https://unity3d.com/kr/get-unity/download/archive
위 경로에서 환경(OS)에 맞춰 설치합니다. (반드시 2018.4.6f1 버전을 선택해 설치하셔야 합니다.)
```

2. node.js 설치 (with npm)

```
https://nodejs.org/ko/
위 경로에서 사용하시는 환경(OS)에 맞춰 설치합니다.(npm도 같이 설치가 됩니다.)
```

3. 샘플 소스 다운로드

```bash
$ wget https://github.com/play2bitgit/Play2Bit-Platform-Sample-LoginTest/raw/master/dist/Play2Bit-Platform-Sample.zip
```

4. Login-Test App 프로젝트 열기

```
유니티에서 프로젝트를 열기를 선택해 Play2Bit-Platform-SampleApp\sample 경로로 이동해서 프로젝트를 열어줍니다.
```

5. Login-Test App 빌드

```
파일 > 빌드 설정 > Android
```

6. Login-Test App 설치 및 실행

```
위(5번)에서 빌드 후 생성된 설치 파일(*.apk)을 가지고 계신 안드로이드폰이나 시뮬레이터 등에 복사하시고 설치 후 실행하실 수 있습니다.
위(3번)에서 받은 샘플 소스에 미리 빌드한 설치 파일(*.apk)이 있어 위 과정(4 ~ 5번)없이 설치 및 실행이 가능합니다.
```

7. Login-Test Server 실행

```bash
$ npm start
```

```
실행되어 있는 서버는 CTRL+C를 이용하시거나 서버가 실행된 터미널 창을 닫으시면 종료하실 수 있습니다.
서버 기본 주소는 http://localhost:80/입니다. 포트 변경이 필요한 경우 아래 경로에서 수정하실 수 있습니다.

  Play2Bit-Platform-SampleServer\auth-sample-server\bin\www (Line : 14)
  var port = normalizePort(process.env.PORT || '80');   // 80을 원하는 포트로 변경

```
