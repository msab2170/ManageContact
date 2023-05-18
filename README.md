# AddressManager

회사들의 목록과 그 직원들의 연락책을 제공하는 사이트

사용 도구 : asp.net7.0 core / sql server / entity framework core (ORM)

ORM은 쓰고 있지만 요구사항 중 Read를 제외하고 storedprocedure로 만들어 달라는 요청이 있어 
회사의 등록, 삭제, 복구과
직원의 등록, 삭제, 복구는 프로시저를 호출하는 방식

1차 완성본을 commit만하고 push를 안하고 와서.... 추후...에 수정하여 보강예정

# 회원 가입
![image](https://github.com/msab2170/ManageContact/assets/115135514/8e47c0c5-3f3d-4470-85a8-fbbb243a989e)

# 로그인
![image](https://github.com/msab2170/ManageContact/assets/115135514/c7bbdc44-36c9-48e5-8d17-cbc446377127)

# 로그인 후 화면 
( 여기서 회원 - 은 연락처들을 관리해주는 사이트의 관리자에 해당한다는 가정으로, 삭제된 회사, 직원목록도 접근이 가능하다.)
![image](https://github.com/msab2170/ManageContact/assets/115135514/04bdbb65-961a-421b-af39-1883b9a77469)

# 로그인 후 회사 목록 
비로그인때와 다르게 회사를 등록할 수 있는 기능을 사용 가능
![image](https://github.com/msab2170/ManageContact/assets/115135514/67554e5f-7558-4289-b104-4622ddd32aec)

# 위 스크린샷에서 create new 버튼 클릭시 회사 등록 가능
![image](https://github.com/msab2170/ManageContact/assets/115135514/6bfc82c4-cecd-4dce-b399-722e3736335c)

# 등록 후 리스트에 추가됨을 확인
![image](https://github.com/msab2170/ManageContact/assets/115135514/a52d2b44-ea08-4ed5-bb8b-ee7f7e3871c7)

# delete 하기
![image](https://github.com/msab2170/ManageContact/assets/115135514/36f253be-1e44-465e-8af7-37ab28da9632)

# Company List에서 사라짐을 확인
![image](https://github.com/msab2170/ManageContact/assets/115135514/609d3d5e-2ae6-4215-a2bf-4bb8e81d6200)


# deleted companies 메뉴에서 확인
![image](https://github.com/msab2170/ManageContact/assets/115135514/fa4d7e52-2d2e-43c9-94dc-32c34d7e601d)

# 복원하기
![image](https://github.com/msab2170/ManageContact/assets/115135514/9dce16d1-db41-45f4-a7f0-bc608e09878a)

# deleted companies 메뉴에서 사라짐 확인
![image](https://github.com/msab2170/ManageContact/assets/115135514/0e5842fe-09f3-4762-8e1e-8ca7badee0e9)
