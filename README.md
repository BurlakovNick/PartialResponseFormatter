# PartialResponseFormatter

This library helps build partial response in RESTful API's (like [Google API's](https://developers.google.com/+/web/api/rest/#partial-response))

Library doesn't depend on any specific stack for building API (WebApi, ServiceStack) or specific serializer (JsonNet). You can use it anywhere as you want.

## Roadmap

- [x] Prototype and github repo
- [x] Simple and slow response formatter with reflection
- [ ] FluentAPI for fields declaration
- [ ] Build partial response fields specification from attributes with reflection
- [ ] Fields mapping validation (when available response fields are known)
- [ ] Ad-hoc field specification serialization for passing as URL parameter
- [ ] Fast response formatting with IL-code emitting
- [ ] Add dynamic objects support