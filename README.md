# AddressManager

회사들의 목록과 그 직원들의 연락책을 제공하는 사이트

사용 도구 : asp.net7.0 core / sql server / entity framework core (ORM)

ORM은 쓰고 있지만 요구사항 중 Read를 제외하고 storedprocedure로 만들어 달라는 요청이 있어 
회사의 등록, 삭제, 복구과
직원의 등록, 삭제, 복구는 프로시저를 호출하는 방식

해당프로시저는 파일시스템에 같이 푸시해두었으므로 참고 바랍니다.

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

# Worker List 메뉴
    ![image](https://github.com/msab2170/ManageContact/assets/115135514/e0ead268-2b40-46c0-adea-93ef150532ac)

# worker 생성
    ![image](https://github.com/msab2170/ManageContact/assets/115135514/ced51902-bb17-46d9-baef-748589b33954)

# 회사는 콤보박스
    ![image](https://github.com/msab2170/ManageContact/assets/115135514/673c2895-f3fc-47cb-8d63-729bb7b85341)

# company1의 직원 등록 완료
    ![image](https://github.com/msab2170/ManageContact/assets/115135514/db7655c0-bfe3-4533-8dfe-6a90574b0201)

    # worker도 삭제/ 복구가 가능(동작 방식이 company와 동일 하므로 스샷 미첨부)

    # 또한 worker가 있는 company는 삭제를 하게되면 회사 안에 있는 worker도 모두 삭제 되도록 개발,
    worker가 모두 없는 company만 삭제할 수 있도록 하는 것도 참조 무결성을 지킬 수 있음..

    # worker 의 details 에서는 등록, 삭제, 복원의 경우 로그 테이블 처럼 볼 수 있도록 개발
    (Act가 C이면 Create - 등록, D이면 Delete - 삭제, R이면 Restore - 복원)
    ![image](https://github.com/msab2170/ManageContact/assets/115135514/62aeecae-12bc-41d2-a2d6-4123b4836654)





